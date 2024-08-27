using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using System.Collections.Concurrent;
using System.Data;

namespace JIYUWU.Core.Common
{
    /// <summary>
    /// 通过内置队列异步定时写日志
    /// </summary>
    public static class Logger
    {
        public static ConcurrentQueue<Base_Log> loggerQueueData = new ConcurrentQueue<Base_Log>();
        private static DateTime lastClearFileDT = DateTime.Now.AddDays(-1);
        private static string _loggerPath = AppSetting.DownLoadPath + "Logger\\Queue\\";
        static Logger()
        {
            Task.Run(() =>
            {
                Start();
            });
        }

        public static void Info(string message)
        {
            Info(LoggerType.Info, message);
        }
        public static void Info(LoggerType loggerType, string message = null)
        {
            Info(loggerType, message, null, null);
        }
        public static void Info(LoggerType loggerType, string requestParam, string resposeParam, string ex = null)
        {
            Add(loggerType, requestParam, resposeParam, ex, LoggerStatus.Info);
        }

        public static void OK(string message)
        {
            OK(LoggerType.Success, message);
        }
        public static void OK(LoggerType loggerType, string message = null)
        {
            OK(loggerType, message, null, null);
        }
        public static void OK(LoggerType loggerType, string requestParam, string resposeParam, string ex = null)
        {
            Add(loggerType, requestParam, resposeParam, ex, LoggerStatus.Success);
        }
        public static void Error(string message)
        {
            Error(LoggerType.Error, message);
        }
        public static void Error(LoggerType loggerType, string message)
        {
            Error(loggerType, message, null, null);
        }
        public static void Error(LoggerType loggerType, string requestParam, string resposeParam, string ex = null)
        {
            Add(loggerType, requestParam, resposeParam, ex, LoggerStatus.Error);
        }
        /// <summary>
        /// 多线程调用日志
        /// </summary>
        /// <param name="message"></param>
        public static void AddAsync(string message, string ex = null)
        {
            AddAsync(LoggerType.Info, null, message, ex, ex != null ? LoggerStatus.Error : LoggerStatus.Info);
        }
        public static void AddAsync(LoggerType loggerType, string requestParameter, string responseParameter, string ex, LoggerStatus status)
        {
            var log = new Base_Log()
            {
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now,
                User_Id = 0,
                UserName = "",
                //  Role_Id = ,
                LogType = loggerType.ToString(),
                ExceptionInfo = ex,
                RequestParameter = requestParameter,
                ResponseParameter = responseParameter,
                Success = (int)status
            };
            loggerQueueData.Enqueue(log);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestParameter">请求参数</param>
        /// <param name="responseParameter">响应参数</param>
        /// <param name="success">响应结果1、成功,2、异常，0、其他</param>
        /// <param name="userInfo">用户数据</param>
        public static void Add(LoggerType loggerType, string requestParameter, string responseParameter, string ex, LoggerStatus status)
        {
            Add(loggerType.ToString(), requestParameter, responseParameter, ex, status);
        }

        public static void Add(string loggerType, string requestParameter, string responseParameter, string ex, LoggerStatus status)
        {
            Base_Log log = null;
            try
            {
                Microsoft.AspNetCore.Http.HttpContext context = HttpContext.Current;
                if (context.Request.Method == "OPTIONS") return;
                ActionObserver cctionObserver = (context.RequestServices.GetService(typeof(ActionObserver)) as ActionObserver);
                if (context == null)
                {
                    WriteText($"未获取到httpcontext信息,type:{loggerType},reqParam:{requestParameter},respParam:{responseParameter},ex:{ex},success:{status.ToString()}");
                    return;
                }
                UserInfo userInfo = UserContext.Current.UserInfo;
                log = new Base_Log()
                {
                    //Id = Guid.NewGuid().ToString(),
                    BeginDate = cctionObserver.RequestDate,
                    EndDate = DateTime.Now,
                    User_Id = userInfo.User_Id,
                    UserName = userInfo.UserTrueName,
                    Role_Id = userInfo.Role_Id,
                    LogType = loggerType,
                    ExceptionInfo = ex,
                    RequestParameter = requestParameter,
                    ResponseParameter = responseParameter,
                    Success = (int)status
                };
                SetServicesInfo(log, context);
            }
            catch (Exception exception)
            {
                log = log ?? new Base_Log()
                {
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    LogType = loggerType.ToString(),
                    RequestParameter = requestParameter,
                    ResponseParameter = responseParameter,
                    Success = (int)status,
                    ExceptionInfo = ex + exception.Message
                };
            }
            loggerQueueData.Enqueue(log);
        }

        private static void Start()
        {
            DataTable queueTable = CreateEmptyTable();
            List<Base_Log> list = new List<Base_Log>();
            while (true)
            {
                try
                {
                    if (loggerQueueData.Count() > 0 && list.Count < 500)
                    {
                        loggerQueueData.TryDequeue(out Base_Log log);
                        list.Add(log);
                        continue;
                    }
                    //每1秒写一次数据
                    Thread.Sleep(1000);
                    if (list.Count == 0) { continue; }
                    if (DBType.Name == "PgSql" || DBType.Name == "Kdbndp" || DBType.Name == "DM")
                    {
                        DbManger.BaseDbContext.Insertable(list).ExecuteCommand();
                    }
                    else
                    {
                        DbManger.BaseDbContext.Fastest<Base_Log>().BulkCopy(list);
                    }

                    list.Clear();

                }
                catch (Exception ex)
                {
                    list.Clear();
                    Console.WriteLine($"日志批量写入数据时出错:{ex.Message}");
                    WriteText(ex.Message + ex.StackTrace + ex.Source);
                }
            }
        }

        private static void WriteText(string message)
        {
            try
            {
                FileHelper.WriteFile(_loggerPath + "WriteError\\", $"{DateTime.Now.ToString("yyyyMMdd")}.txt", message + "\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"日志写入文件时出错:{ex.Message}");
            }
        }

        private static void DequeueToTable(DataTable queueTable)
        {
            loggerQueueData.TryDequeue(out Base_Log log);
            DataRow row = queueTable.NewRow();
            if (log.BeginDate == null)
            {
                log.BeginDate = DateTime.Now;
            }
            //  row["Id"] = log.Id;
            row["LogType"] = log.LogType;
            row["RequestParameter"] = log.RequestParameter;
            row["ResponseParameter"] = log.ResponseParameter;
            row["ExceptionInfo"] = log.ExceptionInfo;
            row["Success"] = log.Success ?? -1;
            row["BeginDate"] = log.BeginDate;
            row["EndDate"] = log.EndDate;
            row["ElapsedTime"] = ((DateTime)log.EndDate - (DateTime)log.BeginDate).TotalMilliseconds;
            row["UserIP"] = log.UserIP;
            row["ServiceIP"] = log.ServiceIP;
            row["BrowserType"] = log.BrowserType;
            row["Url"] = log.Url;
            row["User_Id"] = log.User_Id ?? -1;
            row["UserName"] = log.UserName;
            row["Role_Id"] = log.Role_Id ?? -1;
            queueTable.Rows.Add(row);
        }
        private static DataTable CreateEmptyTable()
        {
            DataTable queueTable = new DataTable();
            queueTable.Columns.Add("LogType", typeof(string));
            queueTable.Columns.Add("RequestParameter", typeof(string));
            queueTable.Columns.Add("ResponseParameter", typeof(string));
            queueTable.Columns.Add("ExceptionInfo", typeof(string));
            queueTable.Columns.Add("Success", Type.GetType("System.Int32"));
            queueTable.Columns.Add("BeginDate", Type.GetType("System.DateTime"));
            queueTable.Columns.Add("EndDate", Type.GetType("System.DateTime"));
            queueTable.Columns.Add("ElapsedTime", Type.GetType("System.Int32"));
            queueTable.Columns.Add("UserIP", typeof(string));
            queueTable.Columns.Add("ServiceIP", typeof(string));
            queueTable.Columns.Add("BrowserType", typeof(string));
            queueTable.Columns.Add("Url", typeof(string));
            queueTable.Columns.Add("User_Id", Type.GetType("System.Int32"));
            queueTable.Columns.Add("UserName", typeof(string));
            queueTable.Columns.Add("Role_Id", Type.GetType("System.Int32"));
            return queueTable;
        }

        public static void SetServicesInfo(Base_Log log, Microsoft.AspNetCore.Http.HttpContext context)
        {
            string result = String.Empty;
            log.Url = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase +
                context.Request.Path;

            log.UserIP = context.GetUserIp()?.Replace("::ffff:", "");
            log.ServiceIP = context.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + context.Connection.LocalPort;

            log.BrowserType = context.Request.Headers["User-Agent"];
            if (log.BrowserType != null && log.BrowserType.Length > 190)
            {
                log.BrowserType = log.BrowserType.Substring(0, 190);
            }
            if (string.IsNullOrEmpty(log.RequestParameter))
            {
                try
                {
                    log.RequestParameter = context.GetRequestParameters();
                }
                catch (Exception ex)
                {
                    log.ExceptionInfo += $"日志读取参数出错:{ex.Message}";
                    Console.WriteLine($"日志读取参数出错:{ex.Message}");
                }
            }
        }
    }
    public class ActionObserver
    {
        /// <summary>
        /// 记录action执行的开始时间
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// 当前请求是否已经写过日志,防止手动与系统自动重复写日志
        /// </summary>
        public bool IsWrite { get; set; }

        public Microsoft.AspNetCore.Http.HttpContext HttpContext { get; }
    }
    public enum LoggerStatus
    {
        Success = 1,
        Error = 2,
        Info = 3
    }
}

using JIYUWU.Core.Common;
using JIYUWU.Entity.Base;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.DbSqlSugar
{
    public static class DbRelativeCache
    {
        private static Dictionary<string, Type> DbContextTypes = new Dictionary<string, Type>();
        private static Dictionary<string, Type> DbEntityTypes = new Dictionary<string, Type>();

        public static Dictionary<string, string> DbContextConnection = new Dictionary<string, string>();

        static DbRelativeCache()
        {
            InitDbContextType();
            InitDbEntityType();
        }
        /// <summary>
        /// 缓存分库DbContext
        /// </summary>
        public static void InitDbContextType()
        {
            //系统库链接
            DbContextConnection["BaseDbContext"] = AppSetting.GetSection("ConnectionStrs")["DbConnectionStr"];
            var compilationLibrary = DependencyContext
                 .Default
                 .RuntimeLibraries
                 .Where(x => x.Name.EndsWith(".Core") && !x.Serviceable && x.Type != "package" && x.Type == "project");
            foreach (var _compilation in compilationLibrary)
            {
                //加载指定类
                foreach (var item in AssemblyLoadContext.Default
                .LoadFromAssemblyName(new AssemblyName(_compilation.Name))
                .GetTypes().Where(x => x.GetTypeInfo().BaseType != null
                && x.BaseType == (typeof(BaseDbContext))))
                {
                    DbContextTypes[item.Name] = item;
                    try
                    {
                        if (!DbContextConnection.TryGetValue(item.Name, out string value))
                        {
                            //缓存自定义库及业务库链接
                            DbContextConnection[item.Name] = AppSetting.GetSection("ConnectionStrs")[item.Name];
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"未配置【{item.Name}】链接配置,{ex.Message + ex.StackTrace}");
                    }
                }
            }
        }
        /// <summary>
        /// 缓存分库model基类
        /// </summary>
        public static void InitDbEntityType()
        {
            var compilationLibrary = DependencyContext
                 .Default
                 .CompileLibraries
                 .Where(x => x.Name.EndsWith(".Entity") && !x.Serviceable && x.Type != "package" && x.Type == "project");
            foreach (var _compilation in compilationLibrary)
            {
                //加载指定类
                foreach (var item in AssemblyLoadContext.Default
                .LoadFromAssemblyName(new AssemblyName(_compilation.Name))
                .GetTypes().Where(x => x.GetTypeInfo().BaseType != null
                && x.BaseType == (typeof(BaseEntity))))
                {
                    DbEntityTypes[item.Name] = item;
                }
            }
        }

        /// <summary>
        /// 根据分库名称获取dbcontext
        /// </summary>
        /// <param name="dbService"></param>
        /// <returns></returns>
        public static Type GetDbContextType(string dbService)
        {
            return DbContextTypes[dbService];
        }

        /// <summary>
        /// 根据分库名称获取分库model基类
        /// </summary>
        /// <param name="dbService"></param>
        /// <returns></returns>
        public static Type GetDbEntityType(string dbService)
        {
            Type dbContextType = DbContextTypes[dbService];
            string name = dbContextType.Name.Replace("DbContext", "");
            return DbEntityTypes[$"{name}Entity"];
        }


    }
}

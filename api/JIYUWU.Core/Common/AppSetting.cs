using JIYUWU.Core.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    public static class AppSetting
    {
        public static IConfiguration Configuration { get; private set; }

        public static string DbConnectionString
        {
            get { return _connection.DbConnectionStr; }
        }

        public static string RedisConnectionString
        {
            get { return _connection.RedisConnectionStr; }
        }

        public static bool UseRedis
        {
            get { return _connection.UseRedis; }
        }
        public static bool UseSignalR
        {
            get { return _connection.UseSignalR; }
        }

        public static bool UseSqlserver2008
        {
            get { return _connection.UseSqlServer2008; }
        }
        public static Secret Secret { get; private set; }
        public static CreateMember CreateMember { get; private set; }

        public static ModifyMember ModifyMember { get; private set; }

        private static Connection _connection;

        public static string TokenHeaderName = "Authorization";

        /// <summary>
        /// Actions权限过滤
        /// </summary>
        public static GlobalFilter GlobalFilter { get; set; }

        /// <summary>
        /// JWT有效期(分钟=默认120)
        /// </summary>
        public static int ExpMinutes { get; private set; } = 120;
        public static string FullStaticPath { get; private set; } = null;
        public static string CurrentPath { get; private set; } = null;
        public static string DownLoadPath { get { return CurrentPath + "\\Download\\"; } }
        //使用动态分库
        public static bool UseDynamicShareDB { get; set; }
        //逻辑删除字段(对应表字段，逻辑删除只会将字段的值设置为1,默认是0)
        public static string LogicDelField { get; set; } = null;
        //表的租户字段(使用动态分库功能此字段用不上)
        public static string TenancyField { get; set; } = null;

        /// <summary>
        /// 是否使用雪花算法(表的主键字段为bigint类型时启用雪花算法生成唯一id;)
        /// </summary>
        public static bool UseSnow { get; set; }

        //是否使用用户权限(限制只能看到指定用户创建的数据,用户管理页面的操作列可以看到此功能,设置为1后生效)
        public static bool UserAuth { get; set; }

        //2023.12.25所有静态文件访问授权
        public static bool FileAuth { get; set; }
        public static void Init(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
            services.Configure<Secret>(configuration.GetSection("Secret"));
            services.Configure<Connection>(configuration.GetSection("ConnectionStrs"));
            services.Configure<GlobalFilter>(configuration.GetSection("GlobalFilter"));

            var provider = services.BuildServiceProvider();
            IWebHostEnvironment environment = provider.GetRequiredService<IWebHostEnvironment>();
            CurrentPath = Path.Combine(environment.ContentRootPath, "").ReplacePath();

            Secret = provider.GetRequiredService<IOptions<Secret>>().Value;

            //设置修改或删除时需要设置为默认用户信息的字段
            CreateMember = provider.GetRequiredService<IOptions<CreateMember>>().Value ?? new CreateMember();
            ModifyMember = provider.GetRequiredService<IOptions<ModifyMember>>().Value ?? new ModifyMember();

            GlobalFilter = provider.GetRequiredService<IOptions<GlobalFilter>>().Value ?? new GlobalFilter();

            GlobalFilter.Actions = GlobalFilter.Actions ?? new string[0];

            _connection = provider.GetRequiredService<IOptions<Connection>>().Value;

            FullStaticPath = Configuration.GetSection("VirtualPath:StaticFile").Value;

            LogicDelField = Configuration["LogicDelField"];

            UseSnow = Configuration["UseSnow"]?.ToString() == "1";
            UserAuth = Configuration["UserAuth"]?.ToString() == "1";
            //2023.12.25所有静态文件访问授权
            FileAuth = Configuration["FileAuth"]?.ToString() == "1";
            if (LogicDelField == "")
            {
                LogicDelField = null;
            }

            TenancyField = Configuration["TenancyField"];

            if (TenancyField == "")
            {
                TenancyField = null;
            }
            UseDynamicShareDB = configuration["UseDynamicShareDB"] == "1";

            FullStaticPath = Directory.GetCurrentDirectory() + "\\wwwroot\\lang\\";

            FullStaticPath = FullStaticPath.ReplacePath();
            Console.WriteLine(FullStaticPath);
            if (!Directory.Exists(FullStaticPath))
            {
                Directory.CreateDirectory(FullStaticPath);
            }

            ExpMinutes = (configuration["ExpMinutes"] ?? "120").GetInt();

            DBType.Name = _connection.DBType;
            if (string.IsNullOrEmpty(_connection.DbConnectionStr))
                throw new System.Exception("未配置好数据库默认连接");

            try
            {
                _connection.DbConnectionStr = _connection.DbConnectionStr.DecryptDES(Secret.DB);
            }
            catch { }

            if (!string.IsNullOrEmpty(_connection.RedisConnectionStr))
            {
                try
                {
                    _connection.RedisConnectionStr = _connection.RedisConnectionStr.DecryptDES(Secret.Redis);
                }
                catch { }
            }

        }
        // 多个节点name格式 ：["key:key1"]
        public static string GetSettingString(string key)
        {
            return Configuration[key];
        }
        // 多个节点,通过.GetSection("key")["key1"]获取
        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }

    }
    public class Connection
    {
        public string DBType { get; set; }
        public bool UseSqlServer2008 { get; set; }
        public string DbConnectionStr { get; set; }
        public string RedisConnectionStr { get; set; }
        public bool UseRedis { get; set; }
        public bool UseSignalR { get; set; }
    }
    public class GlobalFilter
    {
        public string Message { get; set; }
        public bool Enable { get; set; }
        public string[] Actions { get; set; }
    }
    public class CreateMember : TableDefaultColumns
    {
    }
    public class ModifyMember : TableDefaultColumns
    {
    }

    public abstract class TableDefaultColumns
    {
        public string UserIdField { get; set; }
        public string UserNameField { get; set; }
        public string DateField { get; set; }
    }
}

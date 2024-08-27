using JIYUWU.Core.Common;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.DbSqlSugar
{
    public static class SqlSugarRegister
    {

        /// <summary>
        ///系统库链接
        /// </summary>
        /// <returns></returns>
        public static ConnectionConfig GetSysConnectionConfig()
        {
            var dbType = DbManger.GetDbType();

            return new ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = DbServerProvider.SysConnectingString,
                IsAutoCloseConnection = true,
                ConfigId = "default",
                MoreSettings = new ConnMoreSettings()
                {
                    PgSqlIsAutoToLower = false,
                    IsAutoToUpper = false,
                },
                ConfigureExternalServices = GetConfigureExternalServices()
            };
        }



        /// <summary>
        ///  模板空库(租户动态分才使用)
        /// </summary>
        /// <returns></returns>
        private static ConnectionConfig GetEmptyConnectionConfig()
        {
            var dbType = DbManger.GetDbType();
            //模板空库(租户动态分才使用)
            return new ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = AppSetting.GetSection("ConnectionStrs")["EmptyDbContext"],
                IsAutoCloseConnection = true,
                ConfigId = "EmptyDbContext",
                MoreSettings = new ConnMoreSettings()
                {
                    PgSqlIsAutoToLower = false,
                    IsAutoToUpper = false
                },
                ConfigureExternalServices = GetConfigureExternalServices()
            };
        }

        public static IServiceCollection UseSqlSugar(this IServiceCollection services)
        {
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            services.AddHttpContextAccessor();

            var dbType = DbManger.GetDbType();

            //缓存所有配置文件的中的数据库链接
            var configs = DbRelativeCache.DbContextConnection
                .Where(x => x.Key.EndsWith("DbContext") || x.Key == "default").Select(s => new ConnectionConfig()
                {
                    //分库使用不同类型的数据库
                    DbType = SqlSugarDbType.GetType(s.Key, dbType),
                    ConnectionString = s.Value,
                    IsAutoCloseConnection = true,
                    ConfigId = s.Key,
                    MoreSettings = new ConnMoreSettings()
                    {
                        PgSqlIsAutoToLower = false,
                        IsAutoToUpper = false
                    },
                    ConfigureExternalServices = GetConfigureExternalServices()
                }).ToList();

            configs.Add(GetEmptyConnectionConfig());
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                var sysConfig = GetSysConnectionConfig();
                SqlSugarScope sqlSugar = new SqlSugarScope(
                configs,
                db =>
                {
                    //业务库日志
                    foreach (var item in configs.Where(x => x.ConfigId?.ToString() != "BaseDbContext"))
                    {
                        string id = item.ConfigId.ToString();
                        if (db.GetConnection(id) != null)
                        {
                            db.GetConnection(id).Aop.OnLogExecuting = (sql, pars) =>
                            {
                                Console.WriteLine(sql);
                            };
                        }
                    };
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql);
                    };

                });
                return sqlSugar;
            });
            return services;
        }

        /// <summary>
        /// 设置字段全大写
        /// </summary>
        /// <returns></returns>
        private static ConfigureExternalServices GetConfigureExternalServices()
        {
            return new ConfigureExternalServices()
            {
                EntityService = (property, column) =>
                {
                    if (DBType.Name == "DM")
                    {
                        column.DbColumnName = property.Name.ToUpper();
                    }
                }
            };
        }

    }
}

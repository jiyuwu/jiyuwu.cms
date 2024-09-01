using JIYUWU.Base.IRepository;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace JIYUWU.Base.Service
{
    public partial class Base_DbServiceService {

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBase_DbServiceRepository _repository;//访问数据库

    [ActivatorUtilitiesConstructor]
    public Base_DbServiceService(
        IBase_DbServiceRepository dbRepository,
        IHttpContextAccessor httpContextAccessor
        )
    : base(dbRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _repository = dbRepository;
        //多租户会用到这init代码，其他情况可以不用
        //base.Init(dbRepository);
    }

    WebResponseContent webResponse = new WebResponseContent();

    public WebResponseContent CreateDb(string id)
    {
        var item = DbCache.GetDbInfo(id);
        try
        {
            if (item == null)
            {
                return webResponse.Error("请配置数据库名、ip地址、帐号与密码");
            }
            string connectionString = DbCache.InitConnection(item, "master");
            var dapper = DbServerProvider.Db; //DBServerProvider.GetServiceDb(item.DbServiceId);//DBServerProvider.GetSqlDapper(item.DbServiceId.ToString());
            string sql = " select name from sys.databases where name = @name";
            var _dbName = dapper.ExecuteScalar(sql, new { name = item.DatabaseName });
            if (_dbName != null && _dbName.ToString() != "")
            {
                return webResponse.Error($"【{item.DatabaseName}】已存在");
            }
            sql = GetCopyDbSql(item.DatabaseName);
            dapper.SetTimout(60 * 3).ExcuteNonQuery(sql, new { item.DatabaseName, id });
        }
        catch (Exception ex)
        {
            string message = $"创建数据库异常:ID:{item.DatabaseName},异常信息：{ex.Message}";
            Console.WriteLine(message);
            Logger.Error(message);
            return webResponse.Error(message);
        }
        return webResponse.OK("创建成功");
    }


    private string GetCopyDbSql(string dbName)
    {
        string DBPath = AppSetting.GetSettingString("DBPath");
        string DBBackPath = AppSetting.GetSettingString("DBBackPath");
        string DB_Empty = "DB_Empty";
        string sql = @$"USE [master] 
                CREATE DATABASE [{dbName}]
                 CONTAINMENT = NONE
                 ON  PRIMARY 
                ( NAME = N'{dbName}', FILENAME = N'{DBPath}\{dbName}.mdf' , SIZE = 5120KB , FILEGROWTH = 1024KB )
                 LOG ON 
                ( NAME = N'{dbName}_log', FILENAME = N'{DBPath}\{dbName}_log.ldf' , SIZE = 2048KB , FILEGROWTH = 10%)
                
                ALTER DATABASE [{dbName}] SET COMPATIBILITY_LEVEL = 110
                
                ALTER DATABASE [{dbName}] SET ANSI_NULL_DEFAULT OFF 
                
                ALTER DATABASE [{dbName}] SET ANSI_NULLS OFF 
                
                ALTER DATABASE [{dbName}] SET ANSI_PADDING OFF 
                
                ALTER DATABASE [{dbName}] SET ANSI_WARNINGS OFF 
                
                ALTER DATABASE [{dbName}] SET ARITHABORT OFF 
                
                ALTER DATABASE [{dbName}] SET AUTO_CLOSE OFF 
                
                ALTER DATABASE [{dbName}] SET AUTO_CREATE_STATISTICS ON 
                
                ALTER DATABASE [{dbName}] SET AUTO_SHRINK OFF 
                
                ALTER DATABASE [{dbName}] SET AUTO_UPDATE_STATISTICS ON 
                
                ALTER DATABASE [{dbName}] SET CURSOR_CLOSE_ON_COMMIT OFF 
                
                ALTER DATABASE [{dbName}] SET CURSOR_DEFAULT  GLOBAL 
                
                ALTER DATABASE [{dbName}] SET CONCAT_NULL_YIELDS_NULL OFF 
                
                ALTER DATABASE [{dbName}] SET NUMERIC_ROUNDABORT OFF 
                
                ALTER DATABASE [{dbName}] SET QUOTED_IDENTIFIER OFF 
                
                ALTER DATABASE [{dbName}] SET RECURSIVE_TRIGGERS OFF 
                
                ALTER DATABASE [{dbName}] SET  DISABLE_BROKER 
                
                ALTER DATABASE [{dbName}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
                
                ALTER DATABASE [{dbName}] SET DATE_CORRELATION_OPTIMIZATION OFF 
                
                ALTER DATABASE [{dbName}] SET PARAMETERIZATION SIMPLE 
                
                ALTER DATABASE [{dbName}] SET READ_COMMITTED_SNAPSHOT OFF 
                
                ALTER DATABASE [{dbName}] SET  READ_WRITE 
                
                ALTER DATABASE [{dbName}] SET RECOVERY FULL 
                
                ALTER DATABASE [{dbName}] SET  MULTI_USER 
                
                ALTER DATABASE [{dbName}] SET PAGE_VERIFY CHECKSUM  
                
                ALTER DATABASE [{dbName}] SET TARGET_RECOVERY_TIME = 0 SECONDS 
                
                IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [{dbName}] MODIFY FILEGROUP [PRIMARY] DEFAULT
                 
                USE [master]
               --备份数据库
               BACKUP DATABASE [DB_Empty] TO  DISK = N'{DBBackPath}\DB_Empty.bak' WITH  COPY_ONLY, NOFORMAT, INIT,
			   NAME = N'{DB_Empty}', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
                DECLARE @tomdf NVARCHAR(50)=N'{DBPath}\{dbName}.mdf'
                DECLARE @tolog NVARCHAR(50)=N'{DBPath}\{dbName}.ldf'
                  RESTORE DATABASE [{dbName}] FROM  DISK = N'{DBBackPath}\{DB_Empty}.bak' WITH  FILE = 1,
                  MOVE  N'{DB_Empty}' TO @tomdf,  MOVE N'{DB_Empty}_log' TO @tolog,  NOUNLOAD,  REPLACE,  STATS = 5;";

        return sql;
    }

}
        
        
        
        
       
}

using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.Filter;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// 使用 Autofac 替换默认的服务提供程序工厂
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
// 添加Swagger生成器服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 设置 Kestrel 和 IIS 配置
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10485760;
});
builder.WebHost.UseKestrel().UseUrls("http://*:9009");
builder.WebHost.UseIIS();

// 添加服务到容器
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin()
               .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                .AllowAnyHeader().AllowAnyMethod();
            });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers().AddNewtonsoftJson(op =>
{
    op.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    op.SerializerSettings.Converters.Add(new LongCovert());
});
//初始化配置文件
AppSetting.Init(builder.Services, configuration);
//// 调用 ConfigureContainer 方法，注册 Autofac 容器中的自定义模块
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Assuming `Services.AddModule` is an extension method for registering modules
    builder.Services.AddModule(containerBuilder, configuration);
});
builder.Services.UseSqlSugar();

var app = builder.Build();

// 配置中间件使用Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //配置HttpContext
    app.UseStaticHttpContext();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
        c.RoutePrefix = string.Empty; // 将Swagger UI设置为应用的根路径（默认为 /swagger）
    });
}
// 添加跨域中间件
app.UseCors(); // 确保跨域在路由中间件之前被调用

app.UseHttpsRedirection();
app.UseRouting();
// 配置中间件
app.UseMiddleware<TokenMiddleware>();
app.UseAuthorization();
app.MapControllers();


app.Run();


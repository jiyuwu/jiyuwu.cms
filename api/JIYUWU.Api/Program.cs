using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using JIYUWU.Core.DbSqlSugar;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllers();
builder.Services.UseSqlSugar();

var app = builder.Build();

// 配置中间件使用Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
        c.RoutePrefix = string.Empty; // 将Swagger UI设置为应用的根路径（默认为 /swagger）
    });
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();


app.Run();


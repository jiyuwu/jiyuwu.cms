using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.ObjectActionValidator;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// ʹ�� Autofac �滻Ĭ�ϵķ����ṩ���򹤳�
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
// ����Swagger����������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ���� Kestrel �� IIS ����
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10485760;
});
builder.WebHost.UseKestrel().UseUrls("http://*:9009");
builder.WebHost.UseIIS();

// ���ӷ�������
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
builder.Services.UseMethodsModelParameters().UseMethodsGeneralParameters();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers();
//��ʼ�������ļ�
AppSetting.Init(builder.Services, configuration);
// ���� ConfigureContainer ������ע�� Autofac �����е��Զ���ģ��
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Assuming `Services.AddModule` is an extension method for registering modules
    builder.Services.AddModule(containerBuilder, configuration);
});
builder.Services.UseSqlSugar();

var app = builder.Build();

// �����м��ʹ��Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
        c.RoutePrefix = string.Empty; // ��Swagger UI����ΪӦ�õĸ�·����Ĭ��Ϊ /swagger��
    });
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();


app.Run();

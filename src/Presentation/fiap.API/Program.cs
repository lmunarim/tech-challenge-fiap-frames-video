using fiap.Application;
using fiap.Repositories;
using fiap.Services;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using fiap.Domain.Interfaces;
using fiap.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using FFMpegCore;

[assembly: ExcludeFromCodeCoverage]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();


builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = $"FIAP - Tech Challenge - Frames Video V-{Guid.NewGuid()}", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
builder.Services.AddHealthChecks();

// Adiciona injeção de dependência no Application
builder.Services.AddHttpClient();
builder.Services.AddApplicationModule();
builder.Services.AddServicesModule();

builder.Services.AddSingleton<Func<IDbConnection>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("fiap.sqlServer");
    var secretService = sp.GetRequiredService<ISecretManagerService>();

    var secret = secretService.ObterSecret<SecretDbConnect>("dev/fiap/sql-rds").Result;

    if (secret.Host is null)
    {
        Console.WriteLine("Não foi possível recuperar a secret - Console.WriteLine"); ;
        Log.Information("Não foi possível recuperar a secret Serilog");
        throw new Exception("Não foi possível recuperar a secret - Lançada excecao");
    }

    connectionString = connectionString
    .Replace("__server__", secret.Host)
    .Replace("__port__", secret.Port)
    .Replace("__db__", secret.DbInstanceIdentifier)
    .Replace("__userdb__", secret.UserName)
    .Replace("__senha__", secret.Password);

    return () => new SqlConnection(connectionString);
});

builder.Services.AddOpenTelemetry().WithTracing(_ => _
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("frames-video"))
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddOtlpExporter(opt =>
    {
        opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
    }));


builder.Services.AddRepositoriesModule();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP - Tech Challenge V1");
});



app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("api/health");
    endpoints.MapHealthChecks("api/metrics");
    endpoints.MapControllers();
});

Log.Information("Iniciando aplicação");
app.Run();



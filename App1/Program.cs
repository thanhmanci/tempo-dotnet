using App1.Controllers;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;


Log.Logger = new LoggerConfiguration()
  .WriteTo.File(new CompactJsonFormatter(), "./logs/myapp.json")
  .CreateLogger();

Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
var services = builder.Services;
services.AddHttpClient("App3", c =>
{
    string app = "http://app3.monitoring.svc:5003";
    c.BaseAddress = new Uri(app);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });

services.AddHttpClient("App1", c =>
{
    string app = "http://app1.monitoring.svc:5001";
    c.BaseAddress = new Uri(app);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });




services.AddOpenTelemetryTracing((builder) => builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("App1"))
.AddAspNetCoreInstrumentation()
.AddSource(nameof(SendMessageController))
.AddMongoDBInstrumentation()
.AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
.AddConsoleExporter()
.AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://tempo.monitoring.svc:4317"); }));




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

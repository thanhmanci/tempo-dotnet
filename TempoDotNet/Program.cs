using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var services = builder.Services;


services.AddHttpClient("logService", c =>
{
    c.BaseAddress = new Uri("http://logtemposervice.monitoring.svc:5076");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });


services.AddOpenTelemetryTracing(
(builder) => builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("log-app"))
.AddAspNetCoreInstrumentation()
.AddConsoleExporter()
.AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://tempo.monitoring.svc:4317"); }));






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

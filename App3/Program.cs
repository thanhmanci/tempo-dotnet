using App3;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Add services to the container.
var services = builder.Services;

services.AddTransient<ISqlRepository, SqlRepository>();
services.AddTransient<IRabbitRepository, RabbitRepository>();

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



services.AddOpenTelemetryTracing((builder) => builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("App3"))
.AddAspNetCoreInstrumentation()
.AddSource(nameof(RabbitRepository))
.AddMongoDBInstrumentation()
.AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
.AddConsoleExporter()
.AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://tempo.monitoring.svc:4317"); }));




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

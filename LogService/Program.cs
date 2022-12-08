using LogService;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;

services.AddOpenTelemetryTracing(
(builder) => builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("log-app"))
.AddAspNetCoreInstrumentation()
.AddMongoDBInstrumentation()
.AddConsoleExporter()
.AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://tempo.monitoring.svc:4317"); }));


builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));
builder.Services.AddSingleton<BooksService>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

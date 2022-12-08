using LogService;
using LogService.Helpers;
using LogService.Services;
using Microsoft.Data.SqlClient;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Data;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IServiceCollection services = builder.Services;
services.AddDbContext<DataContext>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddScoped<IUserService, UserService>();

services.AddOpenTelemetryTracing(
(builder) => builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("log-app"))
.AddAspNetCoreInstrumentation()
.AddMongoDBInstrumentation()
.AddSqlClientInstrumentation(
        options => options.SetDbStatementForText = true)
    .AddConsoleExporter()
.AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://tempo.monitoring.svc:4317"); }));


builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));
builder.Services.AddSingleton<BooksService>();



var app = builder.Build();
var MyActivitySource = new ActivitySource("LogService");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

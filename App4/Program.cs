using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;


namespace App2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = "redis.monitoring.svc:6379";

                    });

                    services.AddOpenTelemetryTracing(builder =>
                    {
                        var provider = services.BuildServiceProvider();
                        IConfiguration config = provider
                                .GetRequiredService<IConfiguration>();

                        builder.AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .Configure((sp, builder) =>
                            {
                                RedisCache cache = (RedisCache)sp.GetRequiredService<IDistributedCache>();
                                builder.AddRedisInstrumentation(cache.GetConnection());
                            })
                            .AddSource(nameof(Worker))
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("App4"))
                            .AddConsoleExporter()
                            .AddOtlpExporter(opt => { opt.Endpoint = new Uri("http://tempo.monitoring.svc:4317"); });

                });

                });
    }
}

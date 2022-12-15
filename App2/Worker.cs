using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using App2.Helpers;

namespace App2
{
    public class Worker : BackgroundService
    {
        private static readonly ActivitySource Activity = new(nameof(Worker));
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();


        private readonly ILogger<Worker> _logger;


        public Worker()
        {

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Worker>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            StartRabbitConsumer();
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        private void StartRabbitConsumer()
        {

            var factory = new ConnectionFactory { HostName = "rabbitmsgqueue.worldretouch.pro" };
            factory.UserName = "thanhmanci";
            factory.Password = "Vietnam123";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "sample", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var parentContext = Propagator.Extract(default, ea.BasicProperties, ActivityHelper.ExtractTraceContextFromBasicProperties);
                    Baggage.Current = parentContext.Baggage;

                    using (var activity = Activity.StartActivity("Process Message", ActivityKind.Consumer, parentContext.ActivityContext))
                    {

                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        ActivityHelper.AddActivityTags(activity);
                        Console.WriteLine(" [x] Received {0}", message);
                        using (var client = new HttpClient())
                        {
                            try
                            {
                                string app = "http://app3.monitoring.svc:5003";
                                client.BaseAddress = new Uri(app);
                                var result = await client.GetAsync("/sql-to-event?message=" + message);
                                string resultContent = await result.Content.ReadAsStringAsync();
                                _logger.LogInformation(resultContent);
                            }
                            catch
                            {
                            }
                        }
                        _logger.LogInformation("Send Request To sql-to-event: ");
                    }

                };
                channel.BasicConsume(queue: "sample", autoAck: false, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                while (true)
                {
                    Console.ReadLine();
                }
            }

        }


    }
}

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace App2
{
    public class Worker : BackgroundService
    {
        private static readonly ActivitySource Activity = new(nameof(Worker));
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

        private readonly ILogger<Worker> _logger;
        private readonly IDistributedCache _cache;


        public Worker(ILogger<Worker> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
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
                channel.QueueDeclare(queue: "sample_2", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        _logger.LogInformation("Message Received: " + message);

                        if (string.IsNullOrEmpty(message))
                        {
                            _logger.LogInformation("Add item into redis cache");
                            await _cache.SetStringAsync("rabbit.message", message, new DistributedCacheEntryOptions{AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)});
                        }


                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"There was an error processing the message: {ex} ");
                    }
                };
                channel.BasicConsume(queue: "sample_2", autoAck: true, consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                while (true)
                {
                    Console.ReadLine();
                }
            }

        }


    }
}

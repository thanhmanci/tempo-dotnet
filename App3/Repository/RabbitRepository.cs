using Newtonsoft.Json;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace App3
{
    public class RabbitRepository : IRabbitRepository
    {
        private static readonly ActivitySource Activity = new(nameof(RabbitRepository));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        private readonly ILogger<RabbitRepository> _logger;
        private readonly IConfiguration _configuration;

        public RabbitRepository(
            ILogger<RabbitRepository> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void Publish(IEvent evt)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "rabbitmsgqueue.worldretouch.pro" };
                factory.UserName = "thanhmanci";
                factory.Password = "Vietnam123";
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var props = channel.CreateBasicProperties();


                    channel.QueueDeclare(queue: "sample_2",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evt));
                    _logger.LogInformation("Publishing message to queue");

                    channel.BasicPublish(exchange: "",
                        routingKey: "sample_2",
                        basicProperties: props,
                        body: body);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error trying to publish a message", e);
                throw;
            }

        }

    }
}

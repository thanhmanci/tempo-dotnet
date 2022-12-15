using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace App1.Controllers
{
    [ApiController]
    [Route("publish-message")]
    public class SendMessageController : ControllerBase
    {

        private readonly ILogger<SendMessageController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private static readonly ActivitySource Activity = new(nameof(SendMessageController));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public SendMessageController(ILogger<SendMessageController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet(Name = "publish-message")]
        public string Get()
        {
            try
            {

                using (var activity = Activity.StartActivity("RabbitMq Publish", ActivityKind.Producer))
                {

                    var factory = new ConnectionFactory { HostName = "rabbitmsgqueue.worldretouch.pro" };
                    factory.UserName = "thanhmanci";
                    factory.Password = "Vietnam123";
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        var props = channel.CreateBasicProperties();

                        AddActivityToHeader(activity, props);

                        channel.QueueDeclare(queue: "sample", durable: false, exclusive: false, autoDelete: false, arguments: null);

                        var body = Encoding.UTF8.GetBytes("I am app1");

                        _logger.LogInformation("Publishing message to queue");

                        channel.BasicPublish(exchange: "", routingKey: "sample", basicProperties: props, body: body);
                    }
                    return "ok";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error trying to publish a message", e);
                return "failed";
            }
        }

        private void AddActivityToHeader(Activity activity, IBasicProperties props)
        {
            Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", "sample");
        }

        private void InjectContextIntoHeader(IBasicProperties props, string key, string value)
        {
            try
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to inject trace context.");
            }
        }


    }
}
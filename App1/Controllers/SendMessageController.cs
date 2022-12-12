using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace App1.Controllers
{
    [ApiController]
    [Route("publish-message")]
    public class SendMessageController : ControllerBase
    {

        private readonly ILogger<SendMessageController> _logger;
        private readonly IHttpClientFactory _clientFactory;

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
                var factory = new ConnectionFactory { HostName = "rabbitmsgqueue.worldretouch.pro" };
                factory.UserName = "thanhmanci";
                factory.Password = "Vietnam123";
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var props = channel.CreateBasicProperties();
                    channel.QueueDeclare(queue: "sample", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes("I am app1");

                    _logger.LogInformation("Publishing message to queue");

                    channel.BasicPublish(exchange: "", routingKey: "sample", basicProperties: props, body: body);
                }
                return "ok";
            }
            catch (Exception e)
            {
                _logger.LogError("Error trying to publish a message", e);
                return "failed";
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace App1.Controllers
{
    [ApiController]
    [Route("http")]
    public class CallApiApp3Controller : ControllerBase
    {

        private readonly ILogger<CallApiApp3Controller> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public CallApiApp3Controller(ILogger<CallApiApp3Controller> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet(Name = "http")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            var client = _clientFactory.CreateClient("App3");
            var response = await client.GetAsync($"/dummy");

            return response;
        }
    }
}
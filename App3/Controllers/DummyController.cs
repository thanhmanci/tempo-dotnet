using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App3.Controllers
{
    [ApiController]
    [Route("dummy")]
    public class DummyController : ControllerBase
    {

        private readonly ILogger<DummyController> _logger;

        public DummyController(ILogger<DummyController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "http")]
        public ActionResult Get()
        {
            _logger.LogInformation("Request To /dummy App3");
            return StatusCode(200, "ok");
        }
    }
}
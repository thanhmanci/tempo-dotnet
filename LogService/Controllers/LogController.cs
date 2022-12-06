using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace LogService.Controllers
{
    [ApiController]
    [Route("log")]
    public class LogController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpGet("compute")]
        public IEnumerable<WeatherForecast> Compute(int n, int x)
        {
            Console.WriteLine($"Getting weather forecast traceID={Tracer.CurrentSpan.Context.TraceId.ToHexString()}");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
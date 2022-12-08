using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace LogService.Controllers
{
    [ApiController]
    [Route("log")]
    public class LogController : ControllerBase
    {
        private readonly BooksService _booksService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, BooksService booksService)
        {
            _logger = logger;
            _booksService = booksService;
        }

        [HttpGet("compute")]
        public async Task<IEnumerable<WeatherForecast>> ComputeAsync(int n, int x)
        {
            Console.WriteLine($"Getting weather forecast traceID={Tracer.CurrentSpan.Context.TraceId.ToHexString()}");

            var books = await _booksService.GetAsync();
            foreach (var item in books)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine(item.Id);
                Console.WriteLine("==============================================");
            }

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
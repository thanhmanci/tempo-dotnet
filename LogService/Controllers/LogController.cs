using AutoMapper;
using LogService.Entities;
using LogService.Services;
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

        private IUserService _userService;
        private IMapper _mapper;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, BooksService booksService,
        IUserService userService,
        IMapper mapper)
        {
            _logger = logger;
            _booksService = booksService;

            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("compute")]
        public async Task<IEnumerable<WeatherForecast>> ComputeAsync(int n, int x)
        {

            for (int i = 0; i < 10; i++)
            {
                var books = await _booksService.GetAsync();
            }
            var users = _userService.GetAll().ToList<User>();

            for (int i = 0; i < 10; i++)
            {
                _userService.GetAll().ToList<User>();
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
using Microsoft.AspNetCore.Mvc;

namespace App3.Controllers
{
    [ApiController]
    [Route("sql-to-event")]
    public class SqlToEventController : ControllerBase
    {

        private readonly ILogger<SqlToEventController> _logger;
        private readonly ISqlRepository _repository;
        private readonly IRabbitRepository _eventPublisher;

        public SqlToEventController(ISqlRepository repository, IRabbitRepository eventPublisher, ILogger<SqlToEventController> logger)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        [HttpGet(Name = "sql-to-event")]
        public async Task<ActionResult> GetAsync(string message)
        {
            _logger.LogInformation("Request To /sql-to-event App3, message: " + message);
            if (!string.IsNullOrEmpty(message))
            {
                //await _repository.Persist(message);
                _eventPublisher.Publish(new MessagePersistedEvent { Message = message });
            }

            return StatusCode(200, "ok");
        }
    }
}
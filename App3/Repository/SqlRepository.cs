using App3.Controllers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace App3
{
    public class SqlRepository : ISqlRepository
    {
        private readonly IConfiguration _configuration;
        private const string Query = "SELECT * From Applications LIMIT 10;";
        ILogger<SqlRepository> _logger;

        public SqlRepository(IConfiguration configuration, ILogger<SqlRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Persist(string message)
        {
            await using var conn = new SqlConnection(_configuration["SqlDbConnString"]);
            await conn.OpenAsync();
            Random rnd = new Random();
            int loopNumber = rnd.Next(1, 10);
            //Do something more complex
            for (int i = 0; i < loopNumber; i++)
            {
                _logger.LogInformation(Query + "no: " + i);
                await using var cmd = new SqlCommand(Query, conn);
                var res = await cmd.ExecuteScalarAsync();
            }
        }
    }
}

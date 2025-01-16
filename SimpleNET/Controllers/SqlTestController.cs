using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SimpleNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SqlTestController> _logger;

        public SqlTestController(IConfiguration configuration, ILogger<SqlTestController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult TestSqlConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return Ok("SQL Server에 연결되었습니다.");
                }
           } 
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Server 연결 실패. Error Code: {ErrorCode}, State: {State}", sqlEx.Number, sqlEx.State);
                Console.WriteLine("Error: " + sqlEx.Message);
                Console.WriteLine("Error Number: " + sqlEx.Number);
                Console.WriteLine("Error State: " + sqlEx.State);
                Console.WriteLine("Error Class: " + sqlEx.Class);
                Console.WriteLine("Stack Trace: " + sqlEx.StackTrace);
                
                // 상세 오류 정보 출력
                foreach (SqlError error in sqlEx.Errors)
                {
                    Console.WriteLine("Message: " + error.Message);
                    Console.WriteLine("Number: " + error.Number);
                    Console.WriteLine("State: " + error.State);
                    Console.WriteLine("Class: " + error.Class);
                }
                return BadRequest($"SQL Server 연결 오류: {sqlEx.Message}. Error Code: {sqlEx.Number}, State: {sqlEx.State}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "일반 오류 발생");
                return BadRequest($"일반 오류 발생: {ex.Message}");
            }
        }
    }
}

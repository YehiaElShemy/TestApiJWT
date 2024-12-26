using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TestApiJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public MockController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult GetMockData()
        {
            var filePath = Path.Combine(_env.ContentRootPath, "DB_json/data.json");
            var jsonData = System.IO.File.ReadAllText(filePath);
            //   return Content(jsonData, "application/json");
            var users = JsonSerializer.Deserialize<object>(jsonData);
            return Ok(users);
        }
    }
}

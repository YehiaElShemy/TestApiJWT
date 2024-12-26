using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TestApiJWT.Helper;

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
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            var users = ReadUsersFromFile();
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1; // Generate new ID
            users.Add(user);
            WriteUsersToFile(users);
            return Ok(user);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var users = ReadUsersFromFile();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            WriteUsersToFile(users);
            return Ok(user);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var users = ReadUsersFromFile();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            users.Remove(user);
            WriteUsersToFile(users);
            return Ok();
        }


        private string GetJsonFilePath()
        {
            return Path.Combine(_env.ContentRootPath, "DB_json/data.json");
        }

        private List<User> ReadUsersFromFile()
        {
            var filePath = GetJsonFilePath();
            if (!System.IO.File.Exists(filePath))
            {
                // Create an empty file if it doesn't exist
                System.IO.File.WriteAllText(filePath, "{ \"users\": [] }");
            }
            var jsonData = System.IO.File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, List<User>>>(jsonData);
            return data?["users"] ?? new List<User>();
        }
        private void WriteUsersToFile(List<User> users)
        {
            var filePath = GetJsonFilePath();
            var data = new Dictionary<string, List<User>> { { "users", users } };
            var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, jsonData);
        }

    }
}

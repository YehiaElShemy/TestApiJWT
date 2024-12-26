using Microsoft.AspNetCore.Mvc;
using TestApiJWT.Helper;
using TestApiJWT.Services;

namespace TestApiJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MockUserService _service;

        public UsersController(MockUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_service.GetUsers());
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            _service.AddUser(user);
            return Ok(user);
        }
    }



}

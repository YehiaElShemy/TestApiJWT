using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestApiJWT.Helper;
using TestApiJWT.Models;
using TestApiJWT.Services;

namespace TestApiJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService auth;


        public ApplicationDbContext db;

        public AuthController(IAuthService _auth, ApplicationDbContext _db)
        {
            auth = _auth;

            db = _db;
        }
        [Authorize(Roles = "User")]
        [HttpGet("allusers")]
        public async Task<IActionResult> index()
        {
            var data = await db.Users.ToListAsync();
            return Ok(data);
        }
        [Route("RegisterAsync")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await auth.RegisterAsync(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                //return Ok(new {result.Email,result.Message});
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

                return Ok(result);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPost("token")]//login
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await auth.GetTokenAsync(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                if (!string.IsNullOrEmpty(result.RefreshToken))
                    SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

                //return Ok(new {result.Email,result.Message});
                return Ok(result);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpPost("role")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await auth.AddRoleAsync(model);
                if (!result.IsNullOrEmpty())
                    return BadRequest(result);

                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshTokon()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await auth.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return BadRequest(result);
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest("token is required");
            var result=await auth.RevokeTokenAsync(token);
            if(!result)
                return BadRequest("token is in valid");
            return Ok(model);

        }
     

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}

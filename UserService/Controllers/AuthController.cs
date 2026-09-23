using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {


        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public class LoginRequest
        {
            public string UserName { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;
        }




        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Normally validate username/password
            // against your database.

            if (request.UserName != "admin" ||
                request.Password != "Admin@123")
            {
                return Unauthorized("Invalid username or password.");
            }

            var userId = Guid.NewGuid();

            var token = _jwtService.GenerateToken(
                userId,
                request.UserName,
                "Admin");

            return Ok(new
            {
                accessToken = token
            });
        }





    }
}

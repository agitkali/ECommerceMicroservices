using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using UserService.Application.Users.CreateUser;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IMediator _mediator;



        
            [HttpGet]
            public string[] GetUsers()
        {
            // Returning an array of strings
            return new string[] { "Laptop", "Mobile", "Tablet" };
        }



        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            CreateUserCommand command)
        {
            var userId =
                await _mediator.Send(command);

            return Ok(new
            {
                UserId = userId
            });
        }
    }
}

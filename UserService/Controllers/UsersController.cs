using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMediator _mediator;

        // Combine both dependencies into a single constructor to ensure all non-nullable fields are initialized.
        public UsersController(IHttpClientFactory httpClient, IMediator mediator)
        {
            _httpClientFactory = httpClient;
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

        public class OrderDto
        {
            public Guid id { get; set; }

            public Guid UserId { get; set; }

            public decimal TotalAmount { get; set; }

            public string Status { get; set; }

            public DateTime CreatedAtUtc { get; set; }
        }

        //[HttpGet]
        //public async Task<string[]> Get(Guid id)
        //{



        //    var httpClient =
        // _httpClientFactory.CreateClient("OrdersController");

        //    var response = await httpClient.GetAsync(
        //        $"api/orders?id={id}");

        //    response.EnsureSuccessStatusCode();

        //    var result =
        //        await response.Content.ReadFromJsonAsync<string[]>();

        //    return result ?? Array.Empty<string>();
        //}



        [HttpGet]
        public async Task<List<OrderDto>> Get(Guid id)
        {
            //var httpClient = _httpClientFactory.CreateClient("OrdersController");


            //// Get JWT from incoming request
            //var token =
            //    await HttpContext.GetTokenAsync("access_token");

            //if (!string.IsNullOrEmpty(token))
            //{
            //    httpClient.DefaultRequestHeaders.Authorization =
            //        new AuthenticationHeaderValue("Bearer", token);
            //}


            //var response = await httpClient.GetAsync(
            //    $"api/orders?id={id}");

            //response.EnsureSuccessStatusCode();

            //var result =
            //    await response.Content.ReadFromJsonAsync<List<OrderDto>>();

            //if (result is null)
            //{
            //    // You can choose to throw, return a default, or handle as appropriate for your API.
            //    // Here, we throw to indicate a not found result.
            //    throw new InvalidOperationException("Order not found.");
            //}

            var httpClient =
       _httpClientFactory.CreateClient("OrdersController");

            // Get Authorization header from incoming request  --
            // means JWT token internaly geting. that is comming here through gateway/users?id=45435345-56fgh6  wise pass that jwt token that token getting
            //from below code. (where our pass the token to controller -- that controller wise point inside api method wise getting token here ?


            // Get Authorization header from incoming request
            if (Request.Headers.TryGetValue("Authorization", out var authorization))
            {
                // Forward the same JWT to OrderService
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization",
                    authorization.ToString());
            }

            var response =
                await httpClient.GetAsync($"api/orders?id={id}");

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content
                    .ReadFromJsonAsync<List<OrderDto>>();

            if (result is null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            return result;
        }
    }
}

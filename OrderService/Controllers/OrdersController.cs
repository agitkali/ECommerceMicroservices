using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders.CreateOrder;
using MediatR;

namespace OrderService.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(
            IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public string[] GetOrder()
        {
            // Returning an array of strings
            return new string[] { "Laptop", "Mobile", "Tablet" };
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
              CreateOrderCommand command)
        {
            var orderId =
                await _mediator.Send(command);

            return Ok(new
            {
                OrderId = orderId
            });
        }
    }
}

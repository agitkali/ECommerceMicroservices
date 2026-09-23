using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Orders.CreateOrder;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Controllers
{
   
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly OrderDbContext _dbContext;

        public OrdersController(
            IMediator mediator, OrderDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid Id)
        {
            var result = await _dbContext.Orders.Where(x => x.Id == Id).ToListAsync();

            // Returning an array of strings
            if (result.Any())
            {
                return Ok(result);
            }
            return BadRequest();
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

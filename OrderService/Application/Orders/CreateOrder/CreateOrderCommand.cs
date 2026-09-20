using MediatR;

namespace OrderService.Application.Orders.CreateOrder
{
    public record CreateOrderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

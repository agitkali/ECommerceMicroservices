using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Infrastructure.Persistence;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Shared.Contracts;
using Shared.Contracts.Events;

namespace OrderService.Application.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly OrderDbContext _context;

        public CreateOrderCommandHandler(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

            try
            {
                // 1. Create Order

                var order = new Order
                {
                    Id = Guid.NewGuid(),

                    UserId = request.UserId,

                    TotalAmount = request.TotalAmount,

                    Status = "Pending",

                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.Orders.Add(order);


                // 2. Create Event

                var orderCreatedEvent =
                    new OrderCreatedEvent
                    {
                        OrderId = order.Id,

                        UserId = order.UserId,

                        TotalAmount =
                            order.TotalAmount,

                        CreatedDate =
                            order.CreatedAtUtc
                    };


                // 3. Create Outbox Message

                var outboxMessage =
                    new OutboxMessage
                    {
                        Id = Guid.NewGuid(),

                        Type =
                            nameof(OrderCreatedEvent),

                        Payload =
                            JsonSerializer.Serialize(
                                orderCreatedEvent),

                        CreatedDate =
                            DateTime.UtcNow
                    };

                _context.OutboxMessages.Add(
                    outboxMessage);


                // 4. Save BOTH together

                await _context.SaveChangesAsync(
                    cancellationToken);


                // 5. Commit transaction

                await transaction.CommitAsync(
                    cancellationToken);


                return order.Id;
            }

            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}

        // public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        // public async Task<int> CreateOrderAsync(CreateOrderCommand command,  CancellationToken cancellationToken)
        //  {
        //var order = new Order
        //{
        //    UserId = request.UserId,
        //    Status = "Pending",
        //    CreatedAtUtc = DateTime.UtcNow
        //};

        //foreach (var item in request.Items)
        //{
        //    order.Items.Add(new OrderItem
        //    {
        //        ProductId = item.ProductId,
        //        Quantity = item.Quantity,
        //        Price = item.Price
        //    });
        //}

        //order.TotalAmount = order.Items.Sum(
        //    x => x.Quantity * x.Price);

        //_db.Orders.Add(order);

        //await _db.SaveChangesAsync(cancellationToken);

        //return order.Id;
        //await using var transaction =
        //           await _db.Database.BeginTransactionAsync(
        //               cancellationToken);

        //try
        //{
        //    // 1. Create Order
        //    var order = new Order
        //    {
        //        UserId = command.UserId,
        //        TotalAmount = command.TotalAmount,
        //        Status = "Pending",
        //        CreatedAtUtc = DateTime.UtcNow
        //    };

        //    // 2. Create Order Items
        //    foreach (var item in command.Items)
        //    {
        //        order.Items.Add(
        //            new OrderItem
        //            {
        //                ProductId = item.ProductId,
        //                Quantity = item.Quantity,
        //                Price = item.Price
        //            });
        //    }
        //    // 3. Save Order
        //    _db.Orders.Add(order);

        //    await _db.SaveChangesAsync(
        //        cancellationToken);

        // At this point order.Id is generated
        // by SQL Server.

        //// 4. Create RabbitMQ Event
        //var orderEvent = new OrderCreatedEvent
        //{
        //    EventId = Guid.NewGuid(),
        //    OrderId = order.Id,
        //    UserId = order.UserId,
        //    TotalAmount = order.TotalAmount
        //};

        //// 5. Create Outbox Message
        //var outboxMessage = new OutboxMessage
        //{
        //    Id = Guid.NewGuid(),

        //    Type = nameof(OrderCreatedEvent),

        //    Payload = JsonSerializer.Serialize(
        //        orderEvent),

        //    OccurredOnUtc = DateTime.UtcNow
        //};
        //// 6. Save Outbox Message
        //_db.OutboxMessages.Add(outboxMessage);

        //await _db.SaveChangesAsync(
        //    cancellationToken);

        //// 7. Commit transaction
        //await transaction.CommitAsync(
        //    cancellationToken);

        //// 8. Return Order Id
        //return order.Id;
        //    }

        //    catch
        //    {
        //        await transaction.RollbackAsync(
        //            cancellationToken);

        //        throw;
        //    }

        //}
    



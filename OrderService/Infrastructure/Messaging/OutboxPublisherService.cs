
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Common.Interfaces;
using OrderService.Infrastructure.Persistence;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Infrastructure.Messaging
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration     _configuration;
        public OutboxPublisherService(
       IServiceScopeFactory scopeFactory,
       IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(
       CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PublishMessages(
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Outbox error: {ex.Message}");
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
        }

        private async Task PublishMessages(
         CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                    .GetRequiredService<OrderDbContext>();

            var messages = await db.OutboxMessages
                    .Where(x => x.ProcessedDate == null)
                    .OrderBy(x => x.CreatedDate)
                    .Take(20)
                    .ToListAsync(cancellationToken);

            if (!messages.Any())
                return;


            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"]
                    ?? "localhost",

                UserName = _configuration["RabbitMQ:UserName"]
                    ?? "admin",

                Password = _configuration["RabbitMQ:Password"]
                    ?? "Admin@123"
            };

            await using var connection =
                await factory.CreateConnectionAsync();

            await using var channel =
                await connection.CreateChannelAsync();


            await channel.ExchangeDeclareAsync(
                "ecommerce.exchangez",
                ExchangeType.Direct,
                durable: true);


            await channel.QueueDeclareAsync(
                "payment.order-createdz",
                durable: true,
                exclusive: false,
                autoDelete: false);


            await channel.QueueBindAsync(
                "payment.order-createdz",
                "ecommerce.exchangez",
                "order.created");


            foreach (var message in messages)
            {
                //var body =
                //    Encoding.UTF8.GetBytes(
                //        message.Payload);

                //await channel.BasicPublishAsync(
                //    exchange: "ecommerce.exchangez",
                //    routingKey: "order.created",
                //    body: body);


                //message.ProcessedDate =
                //    DateTime.UtcNow;


                var body = Encoding.UTF8.GetBytes(message.Payload);

                var properties = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: "ecommerce.exchangez",
                    routingKey: "order.created",
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                message.ProcessedDate = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(
                cancellationToken);
        }


    }
}

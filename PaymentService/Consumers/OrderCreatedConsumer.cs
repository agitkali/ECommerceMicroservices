using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain;
using PaymentService.Infrastructure.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace PaymentService.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {

        private readonly IServiceScopeFactory
       _scopeFactory;

        private readonly IConfiguration
            _configuration;

        public OrderCreatedConsumer(
       IServiceScopeFactory scopeFactory,
       IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }
        //public async Task Consume(OrderCreatedEvent message)
        //{
        //    // Process payment
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new ConnectionFactory
            {
                //HostName =
                //_configuration["RabbitMQ:HostName"]
                //?? "localhost",

                //UserName =
                //_configuration["RabbitMQ:UserName"]
                //?? "guest",

                //Password =
                //_configuration["RabbitMQ:Password"]
                //?? "guest"
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "Admin@123",
                VirtualHost = "/"
            };

            var connection =
                await factory.CreateConnectionAsync();

            var channel =
                await connection.CreateChannelAsync();


            await channel.ExchangeDeclareAsync(
                "ecommerce.exchangez",
                ExchangeType.Direct,
                durable: true);

            await channel.QueueDeclareAsync(
        queue: "payment.order-createdz",
        durable: true,
        exclusive: false,
        autoDelete: false);


            //await channel.QueueDeclareAsync(
            //    "payment.order-created",
            //    durable: true,
            //    exclusive: false,
            //    autoDelete: false);


            //await channel.QueueBindAsync(
            //    "payment.order-created",
            //    "ecommerce.exchange",
            //    "order.created");

            await channel.QueueBindAsync(
            queue: "payment.order-createdz",
            exchange: "ecommerce.exchangez",
            routingKey: "order.created");


            var consumer =
                new AsyncEventingBasicConsumer(channel);


            consumer.ReceivedAsync +=
           async (_, eventArgs) =>
           {
               try
               {
                   // 1. Convert RabbitMQ bytes to string
                   var json =
                       Encoding.UTF8.GetString(
                           eventArgs.Body.ToArray());

                   Console.WriteLine(
                   $"Received message: [{json}]");

                   // 2. Validate empty message
                   if (string.IsNullOrWhiteSpace(json))
                   {
                       Console.WriteLine(
                           "Received empty RabbitMQ message.");

                       await channel.BasicNackAsync(
                           eventArgs.DeliveryTag,
                           multiple: false,
                           requeue: false);

                       return;
                   }

                  
                   var orderEvent =
                   JsonSerializer.Deserialize<OrderCreatedEvent>(
                       json,
                       new JsonSerializerOptions
                       {
                           PropertyNameCaseInsensitive = true
                       });



                 
                   if (orderEvent == null)
                   {
                       Console.WriteLine(
                           "Could not deserialize OrderCreatedEvent.");

                       await channel.BasicNackAsync(
                           eventArgs.DeliveryTag,
                           multiple: false,
                           requeue: false);

                       return;
                   }

                   Console.WriteLine(
                   $"Order received: {orderEvent.OrderId}");

                   
                   // 4. Create DI scope
                   using var scope =
                       _scopeFactory.CreateScope();

                   var db =
                       scope.ServiceProvider
                           .GetRequiredService<PaymentDbContext>();




                   // Idempotency check

                   // 5. Check duplicate message
                   var existingPayment =
                       await db.Payments
                           .FirstOrDefaultAsync(
                               x => x.OrderId == orderEvent.OrderId);

                   if (existingPayment != null)
                   {
                       Console.WriteLine(
                           $"Payment already exists for Order {orderEvent.OrderId}");

                       await channel.BasicAckAsync(
                           eventArgs.DeliveryTag,
                           multiple: false);

                       return;
                   }

                   // 6. Create Payment
                   var payment = new Payment
                   {
                       Id = Guid.NewGuid(),
                       OrderId = orderEvent.OrderId,
                       Amount = orderEvent.TotalAmount,
                       Status = "Pending",
                       CreatedDate = DateTime.UtcNow
                   };

                   db.Payments.Add(payment);

                   // 7. Save Payment
                   await db.SaveChangesAsync();

                   Console.WriteLine(
                       $"Payment created for Order {orderEvent.OrderId}");

                   // 8. ACK only after DB save succeeds
                   await channel.BasicAckAsync(
                       eventArgs.DeliveryTag,
                       multiple: false);

                   Console.WriteLine(
                       $"Message acknowledged for Order {orderEvent.OrderId}");

               }

               catch (Exception ex)
               {
                   Console.WriteLine(
                       $"Payment error: {ex.Message}");

                   await channel.BasicNackAsync(
                       eventArgs.DeliveryTag,
                       false,
                       true);
               }
           };

            // Start consuming
            await channel.BasicConsumeAsync(
                queue: "payment.order-createdz",
                autoAck: false,
                consumer: consumer);

            Console.WriteLine(
                "PaymentService is listening on payment.queue...");

            // Keep BackgroundService alive
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
    }


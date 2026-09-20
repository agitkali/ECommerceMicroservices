using OrderService.Application.Common;
using OrderService.Application.Common.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Infrastructure.Messaging
{
    public class RabbitMqPublisher
    //: IRabbitMqPublisher
    {
        //private readonly IConfiguration _configuration;

        //public RabbitMqPublisher(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //public async Task PublishAsync(string eventType, string message, CancellationToken cancellationToken)
        //{
        //    var factory = new ConnectionFactory
        //    {
        //        HostName = _configuration["RabbitMQ:HostName"]
        //               ?? "localhost",

        //        Port = int.Parse(
        //        _configuration["RabbitMQ:Port"]
        //        ?? "5672"),

        //        UserName = _configuration["RabbitMQ:UserName"]
        //               ?? "guest",

        //        Password = _configuration["RabbitMQ:Password"]
        //               ?? "guest"
        //    };
        //    await using var connection =
        //   await factory.CreateConnectionAsync();

        //    await using var channel =
        //        await connection.CreateChannelAsync();

        //    await channel.ExchangeDeclareAsync(
        //        "ecommerce.events",
        //        ExchangeType.Topic,
        //        durable: true);

        //    var body =
        //        Encoding.UTF8.GetBytes(message);

        //    await channel.BasicPublishAsync(
        //        "ecommerce.events",
        //        eventType,
        //        body);
        //}
        //    //throw new NotImplementedException();
        //}
    }
}



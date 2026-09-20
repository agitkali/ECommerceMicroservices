namespace OrderService.Application.Common.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(string eventType, string message, CancellationToken cancellationToken);
    }
}

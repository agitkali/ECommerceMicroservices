namespace OrderService.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Payload { get; set; } =  string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedDate { get; set; }

        public string? Error { get; set; }
    }
}

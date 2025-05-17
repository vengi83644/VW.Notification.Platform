namespace VW.Notification.Domain.Entities;

public class NotificationEvent
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = string.Empty;

    public Guid CustomerId { get; set; } = Guid.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Dictionary<string, string> Payload { get; set; } = [];

    public NotificationEvent(Guid id, string eventType, Guid customerId, Dictionary<string, string> payload)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty.", nameof(eventType));
        if (payload == null || payload.Count == 0)
            throw new ArgumentException("Payload cannot be empty.", nameof(payload));
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

        Id = id;
        EventType = eventType;
        Payload = payload;
        CustomerId = customerId;
    }
}

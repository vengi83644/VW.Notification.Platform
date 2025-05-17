namespace VW.Notification.Domain.Entities;

public class NotificationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EventId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public NotificationChannel Channel { get; set; }

    public Customer Recipient { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    public DateTime? LastAttemptedTime { get; set; }

    public int RetryCount { get; set; }

    public NotificationStatus NotificationStatus { get; set; }

    public NotificationRequest(Guid eventId, string eventType, NotificationChannel channel, Customer recipient, string body)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

        if (!Enum.IsDefined(typeof(NotificationChannel), channel))
            throw new ArgumentException("Invalid notification channel.", nameof(channel));

        if (recipient == null)
            throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be empty.", nameof(body));

        EventId = eventId;
        EventType = eventType;
        Channel = channel;
        Recipient = recipient;
        Message = body;
        RetryCount = 0;
    }
}

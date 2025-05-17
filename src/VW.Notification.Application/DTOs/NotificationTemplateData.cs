namespace VW.Notification.Application.DTOs;

public class NotificationTemplateData
{
    public Guid EventId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public Customer Recipient { get; set; }

    public NotificationTemplateData(Guid eventId, string eventType, Customer recipient)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

        if (recipient == null)
            throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");

        EventId = eventId;
        EventType = eventType;
        Recipient = recipient;
    }
}

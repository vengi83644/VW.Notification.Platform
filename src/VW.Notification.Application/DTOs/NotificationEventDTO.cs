using System.ComponentModel.DataAnnotations;

namespace VW.Notification.Application.DTOs;

public class NotificationEventDTO
{
    [Required]
    public Guid EventId { get; set; }

    [Required]
    public string EventType { get; set; }

    public Guid CustomerId { get; set; }

    public Dictionary<string, string> Payload { get; set; }
}

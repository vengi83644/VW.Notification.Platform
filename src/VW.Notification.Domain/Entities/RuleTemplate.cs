namespace VW.Notification.Domain.Entities;

public class RuleTemplate
{
    public Guid TemplateId { get; set; }

    public NotificationChannel NotificationChannel { get; set; }

    public RuleTemplate(Guid templateId, NotificationChannel notificationChannel)
    {
        TemplateId = templateId;
        NotificationChannel = notificationChannel;
    }
}

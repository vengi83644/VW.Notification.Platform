namespace VW.Notification.Domain.Entities;

public class RuleTemplate
{
    public string TemplateId { get; set; }

    public NotificationChannel NotificationChannel { get; set; }

    public RuleTemplate(string templateId, NotificationChannel notificationChannel)
    {
        TemplateId = templateId;
        NotificationChannel = notificationChannel;
    }
}

namespace VW.Notification.Application.DTOs;

public class RuleTemplateDTO
{
    public string TemplateId { get; set; }

    public NotificationChannel NotificationChannel { get; set; }

    public RuleTemplate ToRuleTemplate()
    {
        return new RuleTemplate(TemplateId, NotificationChannel);
    }
}

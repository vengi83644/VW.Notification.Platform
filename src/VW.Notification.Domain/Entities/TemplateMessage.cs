namespace VW.Notification.Domain.Entities;

public class TemplateMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TemplateId { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public TemplateMessage(string templateId, string body)
    {
        TemplateId = templateId;
        Body = body;
    }
}

namespace VW.Notification.Application.DTOs;

public class RuleDTO
{
    public Guid Id { get; set; }

    public string RuleName { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public List<RuleTemplateDTO> RuleTemplatesDTO { get; set; } = [];

    public bool IsActive { get; set; }
}

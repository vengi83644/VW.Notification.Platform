namespace VW.Notification.Domain.Entities;

public class Rule
{
    public Guid Id { get; set; }

    public string RuleName { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public List<RuleTemplate> RuleTemplates { get; set; } = [];

    public bool IsActive { get; set; }

    public Rule(Guid id, string ruleName, string eventType, List<RuleTemplate> ruleTemplates, bool isActive)
    {
        Id = id;
        RuleName = ruleName;
        EventType = eventType;
        RuleTemplates = ruleTemplates;
        IsActive = isActive;
    }
}

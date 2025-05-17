namespace VW.Notification.Infrastructure.Persistence.InMemory;

public class InMemoryRuleRepository : IRuleRepository<Rule>
{
    private readonly ILogger<IRuleRepository<Rule>> _logger;
    private readonly List<Rule> _rules = [];

    public InMemoryRuleRepository(ILogger<IRuleRepository<Rule>> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(configuration);

        LoadFromConfig(configuration);
    }

    private void LoadFromConfig(IConfiguration configuration)
    {
        var rulesConfig = configuration.GetSection("Rules").Get<List<RuleDTO>>();

        if (rulesConfig != null)
        {
            foreach (var rule in rulesConfig)
            {
                var ruleTemplateEntity = rule.RuleTemplatesDTO.Select(s => s.ToRuleTemplate()).ToList();

                var ruleEntity = new Rule(rule.Id, rule.RuleName, rule.EventType, ruleTemplateEntity, rule.IsActive);

                Add(ruleEntity);
            }
        }
    }

    public void Add(Rule rule)
    {
        _rules.Add(rule);
    }

    public Rule? GetById(Guid id)
    {
        return _rules.FirstOrDefault(r => r.Id == id && r.IsActive);
    }

    public List<Rule> GetAll()
    {
        return _rules.Where(w => w.IsActive).ToList();
    }
}

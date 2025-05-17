namespace VW.Notification.Infrastructure.Persistence.InMemory;

public class InMemoryCustomerRuleRepository : ICustomerRuleRepository<CustomerRule>
{
    private readonly ILogger<ICustomerRuleRepository<CustomerRule>> _logger;
    private readonly List<CustomerRule> _customerRules = [];

    public InMemoryCustomerRuleRepository(ILogger<ICustomerRuleRepository<CustomerRule>> logger, IConfiguration configuration, ICustomerRepository<Customer> customerRepository)
    {
        _logger = logger;
        ArgumentNullException.ThrowIfNull(configuration);

        LoadFromConfig(configuration);
    }

    private void LoadFromConfig(IConfiguration configuration)
    {
        var customerRulesConfig = configuration.GetSection("Customers").Get<List<CustomerRuleDTO>>();

        if (customerRulesConfig != null)
        {
            foreach (var customerRuleDTO in customerRulesConfig)
            {
                Add(customerRuleDTO.ToCustomerRule());
            }
        }
    }

    public void Add(CustomerRule customerRule)
    {
        _customerRules.Add(customerRule);
    }

    public CustomerRule? GetById(Guid id)
    {
        return _customerRules.FirstOrDefault(c => c.Id == id);
    }

    public List<CustomerRule> GetAll()
    {
        return _customerRules;
    }
}

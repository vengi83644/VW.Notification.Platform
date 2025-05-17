namespace VW.Notification.Infrastructure.Persistence.InMemory;

public class InMemoryCustomerRepository : ICustomerRepository<Customer>
{
    private readonly List<Customer> _customers = [];
    private readonly ILogger<ICustomerRepository<Customer>> _logger;

    public InMemoryCustomerRepository(ILogger<ICustomerRepository<Customer>> logger, IConfiguration configuration)
    {
        _logger = logger;
        ArgumentNullException.ThrowIfNull(configuration);

        LoadFromConfig(configuration);
    }

    private void LoadFromConfig(IConfiguration configuration)
    {
        var customersConfig = configuration.GetSection("Customers").Get<List<CustomerDTO>>();

        if (customersConfig != null)
        {
            foreach (var customerDTO in customersConfig)
            {
                Add(customerDTO.ToCustomer());
            }
        }
    }

    public void Add(Customer customer)
    {
        _customers.Add(customer);
    }

    public IEnumerable<Customer> GetAllCustomers()
    {
        return _customers;
    }

    public Customer? GetById(Guid id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public List<Customer> GetAll()
    {
        return _customers;
    }
}

namespace VW.Notification.Application.DTOs;

public class CustomerRuleDTO
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Dictionary<string, string> Conditions { get; set; }

    public CustomerRule ToCustomerRule()
    {
        return new CustomerRule(Id, CustomerId, Conditions);
    }
}

namespace VW.Notification.Domain.Entities;

public class CustomerRule
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Dictionary<string, string> Conditions { get; set; }

    public CustomerRule(Guid id, Guid customerId, Dictionary<string, string> conditions)
    {
        Id = id;
        CustomerId = customerId;
        Conditions = conditions;
    }
}

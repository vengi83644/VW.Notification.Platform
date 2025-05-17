namespace VW.Notification.Application.DTOs;

public class CustomerDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Customer ToCustomer()
    {
        return new Customer(Id, Name, Email, PhoneNumber);
    }
}

using VW.Notification.Domain.Entities;
using VW.Notification.Domain.Enums;

namespace VW.Notification.Application.Tests;

public class EventServiceTests
{
    [Fact]
    public void ProcessEvent_WhenMatchingRuleFound_ShouldSendNotification()
    {
        var notificationEvent = new NotificationEvent(Guid.NewGuid(), "OrderCreated", new Guid("00000000-0000-0000-0000-000000000001"), new Dictionary<string, string> { { "OrderId", "1" }, { "OrderAmount", "$500" } });

        var customer = new Customer(new Guid("00000000-0000-0000-0000-000000000001"), "John Doe", "john.doe@test.com", "123");

        var ruleTemplates = new List<RuleTemplate>
        {
            new ("OrderCreated_SMS", NotificationChannel.SMS),
            new ("OrderCreated_Email", NotificationChannel.Email),
            new ("OrderShipped_SMS", NotificationChannel.SMS),
            new ("OrderShipped_Email", NotificationChannel.Email)
        };
    }
}

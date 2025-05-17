namespace VW.Notification.Tests.Common;

public static class Faker
{
    public static List<RuleTemplate> RuleTemplates
    {
        get
        {
            return
                [
                new ("OrderCreated_SMS", NotificationChannel.SMS),
                new ("OrderCreated_Email", NotificationChannel.Email),
                new ("OrderShipped_SMS", NotificationChannel.SMS),
                new ("OrderShipped_Email", NotificationChannel.Email)
                ];
        }
    }

    public static List<Rule> Rules
    {
        get
        {
            return
                [
                new (
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    "Send Order Confirmation",
                    "OrderCreated",
                    [.. Faker.RuleTemplates.Where(w=>w.TemplateId.Contains("OrderCreated",StringComparison.OrdinalIgnoreCase) )],
                    true
                    ),

                new (
                    new Guid("00000000-0000-0000-0000-000000000002"),
                    "Send Shipment Notification",
                    "OrderShipped",
                    [.. Faker.RuleTemplates.Where(w=>w.TemplateId.Contains("OrderShipped",StringComparison.OrdinalIgnoreCase) )],
                    true
                    ),

                new (
                    new Guid("00000000-0000-0000-0000-000000000003"),
                    "Send Delivery Confirmation",
                    "OrderDelivered",
                    [.. Faker.RuleTemplates.Where(w=>w.TemplateId.Contains("OrderDelivered",StringComparison.OrdinalIgnoreCase) )],
                    true
                    )
                ];
        }
    }

    public static Dictionary<string, TemplateMessage> Templates
    {
        get
        {
            return new() {
                { "OrderCreated_Email", new TemplateMessage("OrderCreated_Email", "Your order has been created.") },
                { "OrderCreated_SMS", new TemplateMessage("OrderCreated_SMS", "Your order has been created.") },
                { "OrderShipped_SMS", new TemplateMessage("OrderShipped_SMS", "Your order has been shipped.") },
                { "OrderShipped_Email", new TemplateMessage("OrderShipped_Email", "Your order has been shipped.") },
                { "OrderDelivered_SMS", new TemplateMessage("OrderDelivered_SMS", "Your order has been delivered.") },
                { "OrderDelivered_Email", new TemplateMessage("OrderDelivered_Email", "Your order has been delivered.") }
            };
        }
    }
}

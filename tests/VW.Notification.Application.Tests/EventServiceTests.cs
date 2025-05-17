using Moq;
using Xunit;

namespace VW.Notification.Application.Tests;

public class EventServiceTests
{
    private readonly Mock<ILogger<EventService>> _loggerMock;
    private readonly Mock<IRuleRepository<Rule>> _ruleRepositoryMock;
    private readonly Mock<ICustomerRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<ICustomerRuleRepository<CustomerRule>> _customerRuleRepositoryMock;
    private readonly Mock<ITemplateRepository> _templateRepositoryMock;
    private readonly Mock<INotificationStrategyFactory> _notificationStrategyFactoryMock;
    private readonly Mock<ITemplateService<NotificationTemplateData>> _templateServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;

    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _loggerMock = new Mock<ILogger<EventService>>();
        _ruleRepositoryMock = new Mock<IRuleRepository<Rule>>();
        _customerRepositoryMock = new Mock<ICustomerRepository<Customer>>();
        _customerRuleRepositoryMock = new Mock<ICustomerRuleRepository<CustomerRule>>();
        _templateRepositoryMock = new Mock<ITemplateRepository>();
        _notificationStrategyFactoryMock = new Mock<INotificationStrategyFactory>();
        _templateServiceMock = new Mock<ITemplateService<NotificationTemplateData>>();
        _notificationServiceMock = new Mock<INotificationService>();

        _eventService = new EventService(
            _loggerMock.Object,
            _ruleRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _customerRuleRepositoryMock.Object,
            _templateRepositoryMock.Object,
            _notificationStrategyFactoryMock.Object,
            _templateServiceMock.Object
        );
    }

    [Fact]
    public async Task ProcessEvent_WhenMatchingRuleFound_ShouldSendNotificationForEachMatch()
    {
        var notificationEvent = new NotificationEvent(Guid.NewGuid(), "OrderCreated", new Guid("00000000-0000-0000-0000-000000000001"), new Dictionary<string, string> { { "OrderId", "1" }, { "OrderAmount", "$500" } });

        var customer = new Customer(new Guid("00000000-0000-0000-0000-000000000001"), "John Doe", "john.doe@test.com", "123");

        var customerRules = new List<CustomerRule>
        {
            new (
                new Guid ("00000000-0000-0000-0000-000000000001"),
                new Guid("00000000-0000-0000-0000-000000000001"),
                new()
                {
                    {"OrderCreated", "Email,SMS" },
                    {"OrderShipped", "SMS" },
                    {"OrderDelivered", "Email" }
                })
        };

        var customerRuleTemplates = new List<string>();

        foreach (var conditions in customerRules.FirstOrDefault(f => f.CustomerId == customer.Id).Conditions)
        {
            conditions.Value.Split(',').ToList().ForEach(w =>
            {
                if (Enum.TryParse<NotificationChannel>(w.Trim(), true, out var channel))
                {
                    customerRuleTemplates.Add($"{conditions.Key}_{channel}");
                }
            });
        }

        var notifications = customerRuleTemplates.Where(w => w.Contains(notificationEvent.EventType, StringComparison.OrdinalIgnoreCase)).ToList();

        _ruleRepositoryMock.Setup(s => s.GetAll()).Returns(Faker.Rules);

        _customerRepositoryMock.Setup(s => s.GetById(notificationEvent.CustomerId)).Returns(customer);

        _customerRuleRepositoryMock.Setup(s => s.GetById(notificationEvent.CustomerId)).Returns(customerRules.FirstOrDefault(w => w.CustomerId == notificationEvent.CustomerId));

        _templateRepositoryMock.Setup(s => s.GetById($"{notificationEvent.EventType}_{NotificationChannel.Email}")).Returns(Faker.Templates["OrderCreated_Email"]);
        _templateRepositoryMock.Setup(s => s.GetById($"{notificationEvent.EventType}_{NotificationChannel.SMS}")).Returns(Faker.Templates["OrderCreated_SMS"]);

        _templateServiceMock.Setup(s => s.GetTemplateAsync(It.IsAny<string>(), It.IsAny<NotificationTemplateData>())).Returns(Task.FromResult("Dummy Body"));

        _notificationServiceMock.Setup(s => s.SendNotificationAsync(It.IsAny<NotificationRequest>())).Returns(Task.FromResult(true));

        _notificationStrategyFactoryMock.Setup(s => s.GetNotificationService(It.IsAny<NotificationChannel>())).Returns(_notificationServiceMock.Object);

        await _eventService.ProcessEventAsync(notificationEvent);

        _notificationServiceMock.Verify(s => s.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Exactly(notifications.Count));
    }

    [Fact]
    public async Task ProcessEvent_WhenNoMatchingRuleFound_ShouldSendDefaultNotificationToEmail()
    {
        var notificationEvent = new NotificationEvent(Guid.NewGuid(), "OrderCreated", new Guid("00000000-0000-0000-0000-000000000001"), new Dictionary<string, string> { { "OrderId", "1" }, { "OrderAmount", "$500" } });

        var customer = new Customer(new Guid("00000000-0000-0000-0000-000000000001"), "John Doe", "john.doe@test.com", "123");

        var customerRules = new List<CustomerRule>
        {
            new (
                new Guid ("00000000-0000-0000-0000-000000000001"),
                new Guid("00000000-0000-0000-0000-000000000001"),
                []
                )
        };

        var customerRuleTemplates = new List<string>();

        foreach (var conditions in customerRules.FirstOrDefault(f => f.CustomerId == customer.Id).Conditions)
        {
            conditions.Value.Split(',').ToList().ForEach(w =>
            {
                if (Enum.TryParse<NotificationChannel>(w.Trim(), true, out var channel))
                {
                    customerRuleTemplates.Add($"{conditions.Key}_{channel}");
                }
            });
        }

        var notifications = customerRuleTemplates.Where(w => w.Contains(notificationEvent.EventType, StringComparison.OrdinalIgnoreCase)).ToList();

        _ruleRepositoryMock.Setup(s => s.GetAll()).Returns(Faker.Rules);

        _customerRepositoryMock.Setup(s => s.GetById(notificationEvent.CustomerId)).Returns(customer);

        _customerRuleRepositoryMock.Setup(s => s.GetById(notificationEvent.CustomerId)).Returns(customerRules.FirstOrDefault(w => w.CustomerId == notificationEvent.CustomerId));

        _templateRepositoryMock.Setup(s => s.GetById($"{notificationEvent.EventType}_{NotificationChannel.Email}")).Returns(Faker.Templates["OrderCreated_Email"]);
        _templateRepositoryMock.Setup(s => s.GetById($"{notificationEvent.EventType}_{NotificationChannel.SMS}")).Returns(Faker.Templates["OrderCreated_SMS"]);

        _templateServiceMock.Setup(s => s.GetTemplateAsync(It.IsAny<string>(), It.IsAny<NotificationTemplateData>())).Returns(Task.FromResult("Dummy Body"));

        _notificationServiceMock.Setup(s => s.SendNotificationAsync(It.IsAny<NotificationRequest>())).Returns(Task.FromResult(true));

        _notificationStrategyFactoryMock.Setup(s => s.GetNotificationService(It.IsAny<NotificationChannel>())).Returns(_notificationServiceMock.Object);

        await _eventService.ProcessEventAsync(notificationEvent);

        _notificationServiceMock.Verify(s => s.SendNotificationAsync(It.IsAny<NotificationRequest>()), Times.Once);
    }
}

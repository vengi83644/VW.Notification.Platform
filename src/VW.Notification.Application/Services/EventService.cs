using VW.Notification.Application.DTOs;

namespace VW.Notification.Application.Services;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;

    private readonly IRuleRepository<Rule> _ruleRepository;
    private readonly ICustomerRepository<Customer> _customerRepository;
    private readonly ICustomerRuleRepository<CustomerRule> _customerRuleRepository;

    private readonly ITemplateRepository _templateRepository;

    private INotificationService _notificationService;
    private readonly INotificationStrategyFactory _notificationStrategyFactory;

    private readonly ITemplateService<NotificationTemplateData> _templateService;

    public EventService(
        ILogger<EventService> logger,
        IRuleRepository<Rule> ruleRepository,
        ICustomerRepository<Customer> customerRepository,
        ICustomerRuleRepository<CustomerRule> customerRuleRepository,
        ITemplateRepository templateRepository,
        INotificationStrategyFactory notificationStrategyFactory,
        ITemplateService<NotificationTemplateData> templateService)
    {
        _logger = logger;

        _ruleRepository = ruleRepository;
        _customerRepository = customerRepository;
        _customerRuleRepository = customerRuleRepository;

        _templateRepository = templateRepository;

        _notificationStrategyFactory = notificationStrategyFactory;

        _templateService = templateService;
    }

    public async Task ProcessEventAsync(NotificationEvent notificationEvent)
    {
        var rule = _ruleRepository.GetAll().FirstOrDefault(w => w.EventType.Equals(notificationEvent.EventType, StringComparison.OrdinalIgnoreCase));

        if (rule == null)
        {
            _logger.LogWarning("No rule found for event type: {EventType}", notificationEvent.EventType);
            return;
        }

        var customer = _customerRepository.GetById(notificationEvent.CustomerId);

        if (customer == null)
        {
            _logger.LogWarning("No customer found for ID: {CustomerId}", notificationEvent.CustomerId);
            return;
        }

        var customerRule = _customerRuleRepository.GetById(notificationEvent.CustomerId);

        if (customerRule == null)
        {
            _logger.LogWarning("No customer rule found for customer ID: {CustomerId}", notificationEvent.CustomerId);
            return;
        }

        var matchingCustomerPref = customerRule.Conditions.Keys.FirstOrDefault(w => w.Equals(notificationEvent.EventType, StringComparison.OrdinalIgnoreCase));

        var notificationChannels = new List<NotificationChannel> { };

        if (matchingCustomerPref == null)
        {
            _logger.LogWarning("No matching customer preference found for event type: {EventType}. Considering Email by default.", notificationEvent.EventType);
            notificationChannels.Add(NotificationChannel.Email);
        }
        else
        {
            var customerPrefChannels = customerRule.Conditions[matchingCustomerPref];

            customerPrefChannels?.Split(',').ToList().ForEach(w =>
            {
                if (Enum.TryParse<NotificationChannel>(w.Trim(), true, out var channel))
                {
                    notificationChannels.Add(channel);
                }
                else
                {
                    _logger.LogWarning("Invalid notification channel: {Channel}", w.Trim());
                }
            });
        }

        if (notificationChannels.Count != 0)
        {
            foreach (var channel in notificationChannels)
            {
                var templateId = rule.RuleTemplates.FirstOrDefault(w => w.NotificationChannel == channel)?.TemplateId;

                if (string.IsNullOrWhiteSpace(templateId))
                {
                    _logger.LogWarning("No template found for notification channel: {NotificationChannel}", channel);
                    return;
                }

                var template = _templateRepository.GetById(templateId);

                if (template == null)
                {
                    _logger.LogWarning("Template not found in the repository for ID: {TemplateId}", templateId);
                    return;
                }

                var notificationTemplateData = new NotificationTemplateData(notificationEvent.Id, notificationEvent.EventType, customer);

                var notificationMessage = await _templateService.GetTemplateAsync(template?.TemplateId, notificationTemplateData);

                if (notificationMessage == null)
                {
                    _logger.LogWarning("No notification message found for template ID: {TemplateId}", template?.TemplateId);
                    return;
                }

                var notification = new NotificationRequest(notificationEvent.Id, notificationEvent.EventType, channel, customer, notificationMessage);

                _notificationService = _notificationStrategyFactory.GetNotificationService(channel);

                var notificationResult = await _notificationService.SendNotificationAsync(notification);

                if (notificationResult)
                {
                    _logger.LogInformation("Notification sent successfully for event ID: {EventId}", notificationEvent.Id);
                }
                else
                {
                    _logger.LogError("Failed to send notification for event ID: {EventId}.", notificationEvent.Id);
                }
            }
        }
        else
        {
            _logger.LogWarning("No notifications are sent for event type: {EventType} with event id: {EventId}", notificationEvent.EventType, notificationEvent.Id);
        }
    }
}

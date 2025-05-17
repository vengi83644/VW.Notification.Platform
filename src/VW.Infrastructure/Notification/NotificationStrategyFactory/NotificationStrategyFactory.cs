namespace VW.Notification.Infrastructure.Notification.NotificationStrategyFactory;

public class NotificationStrategyFactory : INotificationStrategyFactory
{
    private readonly ILogger<NotificationStrategyFactory> _logger;

    public NotificationStrategyFactory(ILogger<NotificationStrategyFactory> logger)
    {
        _logger = logger;
    }

    private Dictionary<NotificationChannel, INotificationService> NotificationServices
    {
        get
        {
            return new()
            {
                { NotificationChannel.Email, new EmailService(_logger) },
                { NotificationChannel.SMS, new SmsService(_logger) },
                //{ NotificationChannel.PushNotification, new PushNotificationService() },
                //{ NotificationChannel.WhatsApp, new WhatsAppService() },
                //{ NotificationChannel.Slack, new SlackService() },
                //{ NotificationChannel.MicrosoftTeams, new MicrosoftTeamsService() },
                //{ NotificationChannel.Webhook, new WebhookService() }
            };
        }
    }

    public INotificationService GetNotificationService(NotificationChannel channelType)
    {
        return NotificationServices[channelType];
    }
}

using VW.Notification.Infrastructure.RetryPolicies;

namespace VW.Notification.Infrastructure.Notification;

public class EmailService : INotificationService
{
    private readonly ILogger _logger;

    public NotificationChannel ChannelType => NotificationChannel.Email;

    private readonly NotificationRetryPolicy _notificationRetryPolicy;


    public EmailService(ILogger logger)
    {
        _logger = logger;
        _notificationRetryPolicy = new NotificationRetryPolicy(logger);
    }

    public async Task<bool> SendNotificationAsync(NotificationRequest notificationRequest)
    {
        notificationRequest.NotificationStatus = Domain.Enums.NotificationStatus.Retrying;
        notificationRequest.RetryCount++;
        notificationRequest.LastAttemptedTime = DateTime.UtcNow;

        return await _notificationRetryPolicy.ExecuteAsync(notificationRequest, async () =>
        {
            //use an email service provider to send the email

            _logger.LogInformation("--- START EMAIL SEND ---");
            _logger.LogInformation("To: {Recipient}", notificationRequest.Recipient);
            _logger.LogInformation("Body: \n{Body}", notificationRequest.Message);
            _logger.LogInformation("--- EMAIL SENT ---");

        });
    }
}

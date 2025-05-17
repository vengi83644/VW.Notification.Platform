using VW.Notification.Infrastructure.RetryPolicies;

namespace VW.Notification.Infrastructure.Notification;

public class SmsService : INotificationService
{
    private readonly ILogger _logger;

    public NotificationChannel ChannelType => NotificationChannel.Email;

    private readonly NotificationRetryPolicy _notificationRetryPolicy;


    public SmsService(ILogger logger)
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
            //use an sms service provider to send the sms

            _logger.LogInformation("--- START SMS SEND ---");
            _logger.LogInformation("To: {Recipient}", notificationRequest.Recipient);
            _logger.LogInformation("Body: \n{Body}", notificationRequest.Message);
            _logger.LogInformation("--- SMS SENT ---");

        });
    }
}

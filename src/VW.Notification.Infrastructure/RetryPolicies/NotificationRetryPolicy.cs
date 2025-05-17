using Polly;
using Polly.Retry;

namespace VW.Notification.Infrastructure.RetryPolicies;

public class NotificationRetryPolicy
{
    private readonly ILogger _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public Func<Exception, bool> ShouldRetry { get; set; } = ex => ex is TimeoutException || ex is OperationCanceledException;

    public NotificationRetryPolicy(ILogger logger)
    {
        _logger = logger;

        _retryPolicy = Policy
                .Handle<Exception>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timespan, attempt, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {Attempt} for sending notification due to {ExceptionType}. Waiting {Timespan} before next attempt.", attempt, exception.GetType().Name, timespan);
                    }
                );
    }

    public async Task<bool> ExecuteAsync(NotificationRequest notificationRequest, Func<Task<bool>> sendAction)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Attempting to send notification (Event: {EventId}, Channel: {Channel}, Recipient: {Recipient})",
                    notificationRequest.EventId, notificationRequest.Channel, notificationRequest.Recipient);

                await sendAction();

                notificationRequest.NotificationStatus = NotificationStatus.Sent;

                _logger.LogInformation("Notification sent successfully (Event: {EventId}) after attempts.", notificationRequest.EventId);
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification (Event: {EventId}, Channel: {Channel}, Recipient: {Recipient}) after multiple retries.",
                notificationRequest.EventId, notificationRequest.Channel, notificationRequest.Recipient);

            notificationRequest.NotificationStatus = NotificationStatus.Failed;

            notificationRequest.LastAttemptedTime = DateTime.UtcNow;

            return false;
        }
    }
}

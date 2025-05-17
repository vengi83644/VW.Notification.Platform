namespace VW.Notification.Application.Interfaces.Services;

public interface INotificationStrategyFactory
{
    INotificationService GetNotificationService(NotificationChannel channelType);
}

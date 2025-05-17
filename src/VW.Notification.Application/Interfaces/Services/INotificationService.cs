namespace VW.Notification.Application.Interfaces.Services;

public interface INotificationService
{
    Task<bool> SendNotificationAsync(NotificationRequest notificationRequest);
}

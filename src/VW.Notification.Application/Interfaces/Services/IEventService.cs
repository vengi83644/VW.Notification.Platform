namespace VW.Notification.Application.Interfaces.Services;

public interface IEventService
{
    Task ProcessEventAsync(NotificationEvent notificationEvent);
}

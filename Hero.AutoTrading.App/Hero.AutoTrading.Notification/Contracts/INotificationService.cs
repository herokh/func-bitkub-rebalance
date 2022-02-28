using Hero.AutoTrading.Notification.DTOs;

namespace Hero.AutoTrading.Notification.Contracts
{
    public interface INotificationService
    {
        Task PushMessages(NotificationMessage notificationMessage);
    }
}

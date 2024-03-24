using FSH.WebApi.Shared.Notifications;

namespace MovieWebApp.Client.Infrastructure.Notifications;
public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}
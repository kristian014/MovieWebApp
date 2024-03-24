using FSH.WebApi.Shared.Notifications;

namespace MovieWebApp.Client.Infrastructure.Notifications;
public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;
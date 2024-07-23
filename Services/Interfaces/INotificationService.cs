namespace LibraryApi.Services.Interfaces;

public interface INotificationService
{
    Task CheckAndNotifyWaitingCustomersAsync(int bookId);
}
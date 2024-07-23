using LibraryApi.Data;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;

namespace LibraryApi.Services.Implementations;

public class NotificationService(LibraryContext context, ILogger<NotificationService> logger) : BaseService<NotificationService>(context, logger), INotificationService
{
    public async Task SendBookReservationNotificationAsync()
    {
        try
        {
            List<ReservationNotification> notifications = [.. _context.ReservationNotifications.Where(n => !n.IsNotified)];

            foreach (ReservationNotification notification in notifications)
            {
                /*TODO: Call a service to send Email/SMS to customer
                 * Then verify that the notification has been sent
                 * Then set the notification as notified
                */
                if (notification.Customer.PreferredNotificationMethod == EnumNotificationMethod.Email)
                {
                    //Send email
                }
                else
                {
                    //Send SMS
                }

                notification.IsNotified = true;
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending book availability notification.");
        }
    }
}
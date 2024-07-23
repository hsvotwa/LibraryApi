using LibraryApi.DTOs;
using LibraryApi.Entities;

namespace LibraryApi.Services.Interfaces
{
    public interface IBookTransactionService
    {
        Task<GenericResponse<bool>> ReserveBookAsync(int bookId, int customerId);
        Task<GenericResponse<bool>> BorrowBookAsync(int bookId, int customerId);
        Task<GenericResponse<bool>> ReturnBorrowedBookAsync(int bookId);
        Task<GenericResponse<ReservationNotification?>> SaveReservationNotificationAsync(ReservationNotification notification);
        Task<GenericResponse<bool>> DisableNotificationAsync(int notificationId);
        Task<GenericResponse<bool>> CancelReservationAsync(int bookId, int customerId);
        Task<GenericResponse<Book?>> UpdateBookAsync(int id, Book updatedBook);
    }
}
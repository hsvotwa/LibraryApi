using LibraryApi.DTOs;

namespace LibraryApi.Services.Interfaces;

public interface IBookTransactionService
{
    Task<GenericResponse<bool>> ReserveBookAsync(int bookId, int customerId);
    Task<GenericResponse<bool>> BorrowBookAsync(int bookId, int customerId);
    Task<GenericResponse<bool>> ReturnBorrowedBookAsync(int bookId);
    Task<GenericResponse<bool>> SaveReservationNotificationAsync(SetReservationNotificationModel notification);
    Task<GenericResponse<bool>> DisableReservationNotificationAsync(int customerId, int bookId);
    Task<GenericResponse<bool>> CancelReservationAsync(int bookId, int customerId);
}
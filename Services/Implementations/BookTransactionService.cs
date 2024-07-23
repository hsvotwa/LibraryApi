using AutoMapper;
using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryApi.Services.Implementations;

public class BookTransactionService(LibraryContext context, ILogger<BookTransactionService> logger, IBookStatusService bookStatusService, IConfiguration configuration, IMapper mapper, INotificationService notificationService) : BaseService<BookTransactionService>(context, logger), IBookTransactionService
{
    private readonly IBookStatusService _bookStatusService = bookStatusService;
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<GenericResponse<bool>> ReserveBookAsync(int bookId, int customerId)
        => await RecordBookTransactionAsync(bookId, customerId, isReservation: true);

    public async Task<GenericResponse<bool>> BorrowBookAsync(int bookId, int customerId)
        => await RecordBookTransactionAsync(bookId, customerId, isReservation: false);

    public async Task<GenericResponse<bool>> ReturnBorrowedBookAsync(int bookId)
    {
        try
        {
            if (await _context.Books.FindAsync(bookId) is not { } book)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Book not found" };
            }

            BookTransaction? bookTransaction = await _context.BookTransactions
                .FirstOrDefaultAsync(bt => bt.BookId == bookId && bt.BorrowedUntil.HasValue && !bt.ReturnedDate.HasValue);

            if (bookTransaction == null)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "No borrowed record found for this book." };
            }

            bookTransaction.ReturnedDate = DateTime.UtcNow;
            _context.BookTransactions.Update(bookTransaction);
            bool success = await _context.SaveChangesAsync() > 0;

            if (success)
            {
                await _notificationService.CheckAndNotifyWaitingCustomersAsync(bookId);
            }

            return new GenericResponse<bool>
            {
                Response = success,
                Success = success,
                Description = success ? "Book successfully returned" : "Could not return book, please retry."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning the book.");
            return new GenericResponse<bool>
            {
                Response = false,
                Success = false,
                Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
            };
        }
    }

    public async Task<GenericResponse<bool>> SaveReservationNotificationAsync(SetReservationNotificationModel notification)
    {
        try
        {
            if (await _context.Books.FindAsync(notification.BookId) is not { } book)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Book not found" };
            }

            if (await _context.Customers.FindAsync(notification.CustomerId) is not { } customer)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Customer not found" };
            }

            if (await _context.ReservationNotifications.FirstOrDefaultAsync(x => x.CustomerId == notification.CustomerId && x.BookId == notification.BookId && !x.IsNotified) is { } record)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Customer already has an active notification for this book." };
            }

            (BookTransaction? latestRecord, EnumBookStatus bookStatus) = await _bookStatusService.GetLatestBookTransactionAsync(notification.BookId);

            if (latestRecord?.CustomerId == notification.CustomerId)
            {
                return new GenericResponse<bool>
                {
                    Response = false,
                    Success = false,
                    Description = $"Customer already has an active {(bookStatus == EnumBookStatus.Reserved ? "reservation" : "booking")} on this book."
                };
            }

            _context.ReservationNotifications.Add(_mapper.Map<ReservationNotification>(notification));
            bool success = await _context.SaveChangesAsync() > 0;

            return new GenericResponse<bool>
            {
                Response = success,
                Success = success,
                Description = success ? "Notification successfully saved" : "Notification could not be saved"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving notification.");
        }
        return new GenericResponse<bool>
        {
            Response = false,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<bool>> DisableReservationNotificationAsync(int customerId, int bookId)
    {
        try
        {
            if (await _context.ReservationNotifications.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.BookId == bookId && !x.IsNotified) is not { } notification)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "No active notification found for this customer and book." };
            }

            _context.ReservationNotifications.Remove(notification);
            bool success = await _context.SaveChangesAsync() > 0;

            return new GenericResponse<bool>
            {
                Response = success,
                Success = success,
                Description = success ? "Notification successfully disabled" : "Notification could not be disabled"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while disabling notification.");
        }
        return new GenericResponse<bool>
        {
            Response = false,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<bool>> CancelReservationAsync(int bookId, int customerId)
    {
        try
        {
            if (await _context.Books.FindAsync(bookId) is not { } book)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Book not found" };
            }

            if (await _context.Customers.FindAsync(bookId) is not { } customer)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Customer not found" };
            }

            BookTransaction? reservation = await _context.BookTransactions
                .FirstOrDefaultAsync(bt => bt.BookId == bookId && bt.CustomerId == customerId && bt.ReservedUntil.HasValue && bt.ReservedUntil >= DateTime.UtcNow && !bt.BorrowedUntil.HasValue);

            if (reservation == null)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "No active reservation found for this book." };
            }

            _context.BookTransactions.Remove(reservation);
            bool success = await _context.SaveChangesAsync() > 0;

            if (success)
            {
                await _notificationService.CheckAndNotifyWaitingCustomersAsync(bookId);
            }

            return new GenericResponse<bool>
            {
                Response = success,
                Success = success,
                Description = success ? "Reservation successfully cancelled" : "Could not cancel reservation, please retry."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling reservation.");
            return new GenericResponse<bool>
            {
                Response = false,
                Success = false,
                Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
            };
        }
    }

    private async Task<GenericResponse<bool>> RecordBookTransactionAsync(int bookId, int customerId, bool isReservation)
    {
        try
        {
            if (await _context.Books.FindAsync(bookId) is not { } book)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Book not found" };
            }

            if (await _context.Customers.FindAsync(bookId) is not { } customer)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Customer not found" };
            }

            (BookTransaction? latestRecord, EnumBookStatus bookStatus) = await _bookStatusService.GetLatestBookTransactionAsync(bookId);

            switch (bookStatus)
            {
                case EnumBookStatus.Reserved:
                    if (isReservation || latestRecord?.CustomerId != customerId)
                    {
                        return new GenericResponse<bool>
                        {
                            Response = false,
                            Success = false,
                            Description = $"This book is already reserved by {(latestRecord?.CustomerId == customerId ? "this customer. They can borrow it" : "someone else")}."
                        };
                    }
                    break;
                case EnumBookStatus.Borrowed:
                    return new GenericResponse<bool>
                    {
                        Response = false,
                        Success = false,
                        Description = $"This book is already borrowed by {(latestRecord?.CustomerId == customerId ? "you. You cannot borrow it" : "someone else")}."
                    };
            }

            if (latestRecord is null)
            {
                await _context.BookTransactions.AddAsync(new BookTransaction
                {
                    BookId = bookId,
                    CustomerId = customerId,
                    BorrowedUntil = isReservation
                            ? null :
                            DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["LibrarySettings:BookingLengthInDays"])),
                    ReservedUntil = !isReservation
                            ? null :
                            DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["LibrarySettings:ReservationLengthInDays"]))
                });
            }
            else if (latestRecord?.CustomerId == customerId) //Customer is borrowing a book they reserved previously
            {
                latestRecord.BorrowedUntil = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["LibrarySettings:BookingLengthInDays"]));
            }

            bool success = await _context.SaveChangesAsync() > 0;

            return new GenericResponse<bool>
            {
                Response = success,
                Success = success,
                Description = success
                ? $"Book successfully {(isReservation ? "reserved" : "borrowed")}"
                : $"Could not {(isReservation ? "reserve" : "borrow")} book, please retry."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while {(isReservation ? "reserving" : "borrowing")} book.");
        }
        return new GenericResponse<bool>
        {
            Response = false,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }
}
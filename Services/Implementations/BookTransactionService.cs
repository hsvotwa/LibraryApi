using AutoMapper;
using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services.Implementations;

public class BookTransactionService(LibraryContext context, ILogger<BookTransactionService> logger, IBookStatusService bookStatusService, IConfiguration configuration, IMapper mapper) : BaseService<BookTransactionService>(context, logger), IBookTransactionService
{
    private readonly IBookStatusService _bookStatusService = bookStatusService;
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<GenericResponse<bool>> ReserveBookAsync(int bookId, int customerId)
        => await RecordBookTransactionAsync(bookId, customerId, isReservation: true);

    public async Task<GenericResponse<bool>> BorrowBookAsync(int bookId, int customerId)
        => await RecordBookTransactionAsync(bookId, customerId, isReservation: false);

    public async Task<GenericResponse<bool>> ReturnBorrowedBookAsync(int bookId)
    {
        try
        {
            BookTransaction? bookTransaction = await _context.BookTransactions
                .FirstOrDefaultAsync(bt => bt.BookId == bookId && bt.BorrowedUntil.HasValue && !bt.ReturnedDate.HasValue);

            if (bookTransaction == null)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "No borrowed record found for this book." };
            }

            bookTransaction.ReturnedDate = DateTime.UtcNow;
            _context.BookTransactions.Update(bookTransaction);
            bool success = await _context.SaveChangesAsync() > 0;

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

    public async Task<GenericResponse<bool>> DisableNotificationAsync(int notificationId)
    {
        try
        {
            ReservationNotification? notification = await _context.ReservationNotifications.FindAsync(notificationId);

            if (notification == null)
            {
                return new GenericResponse<bool>
                {
                    Response = false,
                    Success = false,
                    Description = "Notification not found"
                };
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
            BookTransaction? reservation = await _context.BookTransactions
                .FirstOrDefaultAsync(bt => bt.BookId == bookId && bt.CustomerId == customerId && bt.ReservedUntil.HasValue && bt.ReservedUntil >= DateTime.UtcNow);

            if (reservation == null)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "No active reservation found for this book." };
            }

            _context.BookTransactions.Remove(reservation);
            bool success = await _context.SaveChangesAsync() > 0;

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

    public async Task<GenericResponse<Book?>> UpdateBookAsync(int id, Book updatedBook)
    {
        try
        {
            Book? existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
            {
                return new GenericResponse<Book?>
                {
                    Response = null,
                    Success = false,
                    Description = "Book not found"
                };
            }

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.ISBN = updatedBook.ISBN;

            _context.Books.Update(existingBook);
            bool success = await _context.SaveChangesAsync() > 0;

            return new GenericResponse<Book?>
            {
                Response = success ? existingBook : null,
                Success = success,
                Description = success ? "Book successfully updated" : "Book could not be updated"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating book.");
        }
        return new GenericResponse<Book?>
        {
            Response = null,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    private async Task<GenericResponse<bool>> RecordBookTransactionAsync(int bookId, int customerId, bool isReservation)
    {
        try
        {
            Book? book = await _context.Books.FindAsync(bookId);

            if (book == null)
            {
                return new GenericResponse<bool> { Response = false, Success = false, Description = "Book not found" };
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
using LibraryApi.Data;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services.Implementations;

public class BookStatusService(LibraryContext context, ILogger<BookStatusService> logger) : BaseService<BookStatusService>(context, logger), IBookStatusService
{
    public async Task<(BookTransaction? record, EnumBookStatus Status)> GetLatestBookTransactionAsync(int bookId)
    {
        BookTransaction? record = await _context.BookTransactions
            .FirstOrDefaultAsync(x => x.BookId == bookId && !x.ReturnedDate.HasValue && (x.ReservedUntil >= DateTime.UtcNow || x.BorrowedUntil >= DateTime.UtcNow));
        return (record, GetBookStatus(record));
    }

    public EnumBookStatus GetBookStatus(BookTransaction? bookTransaction)
    {
        if (bookTransaction is null)
        {
            return EnumBookStatus.Available;
        }

        if (bookTransaction.ReservedUntil >= DateTime.UtcNow && !bookTransaction.BorrowedUntil.HasValue)
        {
            return EnumBookStatus.Reserved;
        }

        if (bookTransaction.BorrowedUntil >= DateTime.UtcNow)
        {
            return EnumBookStatus.Borrowed;
        }

        return EnumBookStatus.Available;
    }
}
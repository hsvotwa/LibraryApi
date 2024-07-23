using LibraryApi.Entities;
using LibraryApi.Utilities;

namespace LibraryApi.Services.Interfaces
{
    public interface IBookStatusService
    {
        EnumBookStatus GetBookStatus(BookTransaction? bookTransaction);
        Task<(BookTransaction? record, EnumBookStatus Status)> GetLatestBookTransactionAsync(int bookId);
    }
}
using LibraryApi.DTOs;
using LibraryApi.Entities;

namespace LibraryApi.Services.Implementations
{
    public interface IBookService
    {
        Task<GenericResponse<Book?>> AddBookAsync(Book book);
        Task<GenericResponse<IEnumerable<BookDetailsDto>>> BookSearchAsync(string searchText);
        Task<GenericResponse<Book?>> GetBookByIdAsync(int id);
        Task<GenericResponse<Book?>> UpdateBookAsync(int id, Book updatedBook);
    }
}
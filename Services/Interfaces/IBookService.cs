using LibraryApi.DTOs;
using LibraryApi.Entities;

namespace LibraryApi.Services.Implementations;

public interface IBookService
{
    Task<GenericResponse<Book?>> AddBookAsync(SetBookModel book);
    Task<GenericResponse<IEnumerable<GetBookModel>>> BookSearchAsync(string searchText);
    Task<GenericResponse<GetBookModel?>> GetBookByIdAsync(int id);
    Task<GenericResponse<Book?>> UpdateBookAsync(int id, SetBookModel updatedBook);
}
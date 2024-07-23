using AutoMapper;
using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services.Implementations;

public class BookService(LibraryContext context, ILogger<BookService> logger, IBookStatusService bookStatusService, IMapper mapper) : BaseService<BookService>(context, logger), IBookService
{
    private readonly IBookStatusService _bookStatusService = bookStatusService;
    private readonly IMapper _mapper = mapper;

    public async Task<GenericResponse<GetBookModel?>> GetBookByIdAsync(int id)
    {
        try
        {
            Book? book = await _context.Books.FindAsync(id);

            (BookTransaction? latestTransaction, EnumBookStatus status) = await _bookStatusService.GetLatestBookTransactionAsync(book.Id);

            var returnBook = _mapper.Map<GetBookModel>(book);

            returnBook = returnBook with
            {
                Status = status,
                ReservedUntil = latestTransaction?.ReservedUntil,
                BorrowedUntil = latestTransaction?.BorrowedUntil
            };
            return new GenericResponse<GetBookModel?> { Response = returnBook };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching book by id.");
        }
        return new GenericResponse<GetBookModel?>
        {
            Response = null,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<IEnumerable<GetBookModel>>> BookSearchAsync(string searchTerm)
    {
        try
        {
            List<Book> books = await _context.Books
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm) || b.ISBN.Contains(searchTerm))
                .Include(b => b.BookTransactions)
                .ToListAsync();

            List<GetBookModel> bookDetailsDtos = [];

            foreach (Book book in books)
            {
                (BookTransaction? latestTransaction, EnumBookStatus status) = await _bookStatusService.GetLatestBookTransactionAsync(book.Id);

                var returnBook = _mapper.Map<GetBookModel>(book);

                returnBook = returnBook with
                {
                    Status = status,
                    ReservedUntil = latestTransaction?.ReservedUntil,
                    BorrowedUntil = latestTransaction?.BorrowedUntil
                };
            }

            return new GenericResponse<IEnumerable<GetBookModel>> { Response = bookDetailsDtos };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching books.");
        }
        return new GenericResponse<IEnumerable<GetBookModel>>
        {
            Response = [],
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<Book?>> AddBookAsync(SetBookModel bookModel)
    {
        try
        {
            var book = _mapper.Map<Book>(bookModel);
            _context.Books.Add(book);
            bool success = await _context.SaveChangesAsync() > 0;

            return new GenericResponse<Book?> { Response = success ? book : null, Success = success, Description = success ? string.Empty : "Book could not be added" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding book.");
        }
        return new GenericResponse<Book?>
        {
            Response = null,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<Book?>> UpdateBookAsync(int id, SetBookModel updatedBook)
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
            existingBook.IsActive = updatedBook.IsActive;

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
}
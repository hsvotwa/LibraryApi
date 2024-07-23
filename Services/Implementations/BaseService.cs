using LibraryApi.Data;

namespace LibraryApi.Services.Implementations;

public class BaseService<T>(LibraryContext context, ILogger<T> logger)
{
    protected readonly ILogger<T> _logger = logger;
    protected readonly LibraryContext _context = context;
}
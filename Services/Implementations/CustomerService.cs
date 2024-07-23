using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services.Implementations;

public class CustomerService(LibraryContext context, ILogger<CustomerService> logger) : BaseService<CustomerService>(context, logger), ICustomerService
{
    public async Task<GenericResponse<Customer?>> GetCustomerByIdAsync(int id)
    {
        try
        {
            Customer? result = await _context.Customers.FindAsync(id);

            return new GenericResponse<Customer?> { Response = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching customer by id.");
        }
        return new GenericResponse<Customer?>
        {
            Response = null,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<IEnumerable<Customer>>> CustomerSearchAsync(string searchText)
    {
        try
        {
            List<Customer> result = await _context.Customers.Where(x => x.Name.Contains(searchText) || x.Email.Contains(searchText) || x.Phone.Contains(searchText)).ToListAsync();

            return new GenericResponse<IEnumerable<Customer>> { Response = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all customers.");
        }
        return new GenericResponse<IEnumerable<Customer>>
        {
            Response = [],
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }

    public async Task<GenericResponse<Customer?>> AddCustomerAsync(Customer customer)
    {
        try
        {
            _context.Customers.Add(customer);
            bool success = await _context.SaveChangesAsync() > 0;

            return new GenericResponse<Customer?> { Response = success ? customer : null, Success = success, Description = success ? string.Empty : "Customer could not be added" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while assing customer.");
        }
        return new GenericResponse<Customer?>
        {
            Response = null,
            Success = false,
            Description = Constants.GENERIC_ERROR_RESPONSE_DESCRIPTION
        };
    }
}
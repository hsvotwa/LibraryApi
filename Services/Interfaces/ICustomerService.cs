using LibraryApi.DTOs;
using LibraryApi.Entities;

namespace LibraryApi.Services.Interfaces;

public interface ICustomerService
{
    Task<GenericResponse<Customer?>> AddCustomerAsync(Customer customer);
    Task<GenericResponse<IEnumerable<Customer>>> CustomerSearchAsync(string searchText);
    Task<GenericResponse<Customer?>> GetCustomerByIdAsync(int id);
}
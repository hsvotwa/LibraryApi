using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
        => Ok(await _customerService.GetCustomerByIdAsync(id));

    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string searchTerm)
        => Ok(await _customerService.CustomerSearchAsync(searchTerm));

    [HttpPost]
    public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
    => Ok(await _customerService.AddCustomerAsync(customer));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        => Ok(await _customerService.UpdateCustomerAsync(id, updatedCustomer));
}
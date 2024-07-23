using LibraryApi.DTOs;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BookTransactionController(IBookTransactionService bookTransactionService) : ControllerBase
{
    private readonly IBookTransactionService _bookTransactionService = bookTransactionService;

    [HttpPost("{bookId}/reserve")]
    public async Task<IActionResult> ReserveBook(int bookId, [FromQuery] int customerId)
        => Ok(await _bookTransactionService.ReserveBookAsync(bookId, customerId));

    [HttpDelete("{bookId}/cancel-reservation/{customerId}")]
    public async Task<IActionResult> CancelReservation(int bookId, int customerId)
        => Ok(await _bookTransactionService.CancelReservationAsync(bookId, customerId));

    [HttpPost("{bookId}/borrow")]
    public async Task<IActionResult> BorrowBook(int bookId, [FromQuery] int customerId)
        => Ok(await _bookTransactionService.BorrowBookAsync(bookId, customerId));

    [HttpPost("{bookId}/return")]
    public async Task<IActionResult> ReturnBorrowedBook(int bookId)
        => Ok(await _bookTransactionService.ReturnBorrowedBookAsync(bookId));

    [HttpPost("notifications/set")]
    public async Task<IActionResult> SaveReservationNotification([FromBody] SetReservationNotificationModel notification)
        => Ok(await _bookTransactionService.SaveReservationNotificationAsync(notification));

    [HttpDelete("notifications/disable/{customerId}/{bookId}")]
    public async Task<IActionResult> DisableReservationNotification(int customerId, int bookId)
        => Ok(await _bookTransactionService.DisableReservationNotificationAsync(customerId, bookId));
}
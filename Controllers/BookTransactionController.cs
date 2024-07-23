using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookBookTransactionController(IBookTransactionService bookTransactionService) : ControllerBase
    {
        private readonly IBookTransactionService _bookTransactionService = bookTransactionService;

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
            => Ok(await _bookTransactionService.UpdateBookAsync(id, updatedBook));

        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveBook(int id, [FromQuery] int customerId)
            => Ok(await _bookTransactionService.ReserveBookAsync(id, customerId));

        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id, [FromQuery] int customerId)
            => Ok(await _bookTransactionService.BorrowBookAsync(id, customerId));

        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBorrowedBook(int id)
            => Ok(await _bookTransactionService.ReturnBorrowedBookAsync(id));

        [HttpPost("notifications")]
        public async Task<IActionResult> SaveReservationNotification([FromBody] ReservationNotification notification)
            => Ok(await _bookTransactionService.SaveReservationNotificationAsync(notification));

        [HttpPost("notifications/disable/{id}")]
        public async Task<IActionResult> DisableNotification(int id)
            => Ok(await _bookTransactionService.DisableNotificationAsync(id));

        [HttpDelete("{bookId}/cancel/{customerId}")]
        public async Task<IActionResult> CancelReservation(int bookId, int customerId)
            => Ok(await _bookTransactionService.CancelReservationAsync(bookId, customerId));
    }
}
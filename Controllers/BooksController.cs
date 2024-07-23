﻿using LibraryApi.Entities;
using LibraryApi.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
            => Ok(await _bookService.GetBookByIdAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm)
            => Ok(await _bookService.BookSearchAsync(searchTerm));

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        => Ok(await _bookService.AddBookAsync(book));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
            => Ok(await _bookService.UpdateBookAsync(id, updatedBook));
    }
}
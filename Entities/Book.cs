using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Entities;

public class Book
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string ISBN { get; set; }
    public bool IsActive { get; set; }

    public virtual required ICollection<BookTransaction> BookTransactions { get; set; }
}
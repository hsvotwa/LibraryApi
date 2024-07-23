using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApi.Entities;

public class BookTransaction
{
    [Key]
    public int Id { get; set; }
    public DateTime? ReservedUntil { get; set; }
    public DateTime? BorrowedUntil { get; set; }
    public DateTime? ReturnedDate { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }
    public virtual Book Book { get; set; }

    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
}
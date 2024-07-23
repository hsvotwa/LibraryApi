using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApi.Entities;

public class ReservationNotification
{
    public int Id { get; set; }
    public bool IsNotified { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }
    public virtual required Book Book { get; set; }

    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual required Customer Customer { get; set; }
}

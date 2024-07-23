namespace LibraryApi.DTOs;

public record SetReservationNotificationModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int CustomerId { get; set; }
}
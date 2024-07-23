using LibraryApi.Utilities;

namespace LibraryApi.DTOs
{
    public class BookDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public EnumBookStatus Status { get; set; }
        public DateTime? ReservedUntil { get; set; }
        public DateTime? BorrowedUntil { get; set; }
    }

}

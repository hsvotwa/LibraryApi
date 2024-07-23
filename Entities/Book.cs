using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public bool IsActive { get; set; }

        public virtual List<BookTransaction> BookTransactions { get; set; }
    }
}
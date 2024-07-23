using LibraryApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<BookTransaction> BookTransactions { get; set; }
        public DbSet<ReservationNotification> ReservationNotifications { get; set; }
    }
}

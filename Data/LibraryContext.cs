using LibraryApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

public class LibraryContext(DbContextOptions<LibraryContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<BookTransaction> BookTransactions { get; set; }
    public DbSet<ReservationNotification> ReservationNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

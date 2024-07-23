using LibraryApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public EnumNotificationMethod PreferredNotificationMethod { get; set; }
}
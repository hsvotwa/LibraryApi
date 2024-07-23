namespace LibraryApi.DTOs;

public record SetBookModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public bool IsActive { get; set; }
}
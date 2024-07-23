namespace LibraryApi.DTOs
{
    public class GenericResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Response { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

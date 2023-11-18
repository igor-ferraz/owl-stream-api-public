namespace OwlStream.Domain.Models.Genres;

public class GenreResult : GenreUpdate
{
    public DateTime? CreationDate { get; set; }
    public bool Active { get; set; }
}
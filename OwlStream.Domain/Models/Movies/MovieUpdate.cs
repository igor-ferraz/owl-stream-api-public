using OwlStream.Domain.Models.Genres;

namespace OwlStream.Domain.Models.Movies;

public class MovieUpdate : Movie
{
    public string Id { get; set; }
    public IEnumerable<GenreUpdate> Genres { get; set; }
}
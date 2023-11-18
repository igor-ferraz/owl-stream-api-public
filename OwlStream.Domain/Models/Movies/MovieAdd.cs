using OwlStream.Domain.Models.Countries;
using OwlStream.Domain.Models.Genres;
using OwlStream.Domain.Models.Languages;

namespace OwlStream.Domain.Models.Movies;

public class MovieAdd : Movie
{
    public IEnumerable<GenreUpdate> Genres { get; set; }
    public IEnumerable<CountryUpdate> Countries { get; set; }
    public IEnumerable<LanguageUpdate> Languages { get; set; }
}
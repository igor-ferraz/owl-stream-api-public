using OwlStream.Domain.Models.Countries;
using OwlStream.Domain.Models.Genres;
using OwlStream.Domain.Models.Languages;

namespace OwlStream.Domain.Models.Movies;

public class MovieResult : Movie
{
    public string Id { get; set; }
    public bool Active { get; set; }
    public DateTime CreationDate { get; set; }
    public IEnumerable<GenreResult> Genres { get; set; }
    public IEnumerable<CountryResult> Countries { get; set; }
    public IEnumerable<LanguageResult> Languages { get; set; }
}
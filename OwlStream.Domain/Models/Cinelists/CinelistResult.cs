using OwlStream.Domain.Models.Movies;

namespace OwlStream.Domain.Models.Cinelists;

public class CinelistResult : CinelistUpdate
{
    public bool Active { get; set; }
    public DateTime CreationDate { get; set; }
    public IEnumerable<MovieResult> Movies { get; set; }
}
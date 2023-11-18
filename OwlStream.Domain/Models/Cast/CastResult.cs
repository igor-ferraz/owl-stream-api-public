using OwlStream.Domain.Models.People;

namespace OwlStream.Domain.Models.Cast;

public class CastResult : Person
{
    public string Id { get; set; }
    public IEnumerable<CastMovieResult> Movies { get; set; }
}
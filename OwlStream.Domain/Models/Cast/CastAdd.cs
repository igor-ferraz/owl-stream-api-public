using OwlStream.Domain.Models.People;

namespace OwlStream.Domain.Models.Cast;

public class CastAdd : Person
{
    public List<CastMovieAdd> Movies { get; set; }
}
using Swashbuckle.AspNetCore.Annotations;

namespace OwlStream.Domain.Models.Movies;

public class Movie
{
    public string Title { get; set; }
    public string Synopsis { get; set; }
    public int DurationInMinutes { get; set; }
    public string Url { get; set; }
    public string PictureUrl { get; set; }

    [SwaggerSchema(Format = "date")]
    public DateTime LaunchDate { get; set; }
}
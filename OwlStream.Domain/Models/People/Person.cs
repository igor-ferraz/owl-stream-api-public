using Swashbuckle.AspNetCore.Annotations;

namespace OwlStream.Domain.Models.People;

public class Person
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public int GenderId { get; set; }

    [SwaggerSchema(Format = "date")]
    public DateTime Birthdate { get; set; }
}
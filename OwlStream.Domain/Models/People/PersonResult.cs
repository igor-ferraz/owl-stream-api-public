namespace OwlStream.Domain.Models.People;

public class PersonResult : PersonUpdate
{
    public Gender Gender { get; set; }
    public DateTime CreationDate { get; set; }
}
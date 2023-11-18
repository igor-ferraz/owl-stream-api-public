using OwlStream.Domain.Models.People;

namespace OwlStream.Domain.Models.Users;

public class UserResult : User
{
    public string Id { get; set; }
    public bool Active { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime ConfirmationDate { get; set; }
    public string PersonId { get; set; }
    public PersonResult Person { get; set; }
}
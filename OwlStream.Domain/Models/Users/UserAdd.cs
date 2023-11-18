using OwlStream.Domain.Models.People;

namespace OwlStream.Domain.Models.Users;

public class UserAdd : User
{
    public string Password { get; set; }
    public Person Person { get; set; }
}
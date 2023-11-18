using OwlStream.Domain.Models.People;

namespace OwlStream.Domain.Models.Users;

public class UserUpdate : User
{
    public Person Person { get; set; }
}
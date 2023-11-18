namespace OwlStream.Domain.Models.Users;

public class UserUpdateInternal : UserUpdate
{
    public UserUpdateInternal(UserUpdate user, string id)
    {
        Id = id;
        Email = user.Email;
        Person = user.Person;
    }

    public string Id { get; set; }
}
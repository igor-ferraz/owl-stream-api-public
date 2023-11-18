namespace OwlStream.Domain.Models.Security;

public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public DateTime ConfirmationDate { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool Active { get; set; }
}
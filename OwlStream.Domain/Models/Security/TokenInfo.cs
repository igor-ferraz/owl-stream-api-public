namespace OwlStream.Domain.Models.Security;

public class TokenInfo
{
    public string Name { get; set; }
    public string Token { get; set; }
    public int ExpirationInMinutes { get; set; }
}
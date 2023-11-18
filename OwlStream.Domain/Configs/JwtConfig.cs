namespace OwlStream.Domain.Configs;

public class JwtConfig
{
    public string Name { get; set; }
    public string SecretKey { get; set; }
    public int ExpirationInMinutes { get; set; }
}
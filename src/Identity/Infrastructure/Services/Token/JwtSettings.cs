namespace IdentityInfrastructure.Services.Token;

public class JwtSettings
{
    public string? PrivateKeyPath { get; set; }
    
    public string? PublicKeyPath { get; set; }

    public string? ExpireTimeAccessTokenInMinute { get; set; }

    public string? ExpireTimeRefreshTokenInDay { get; set; }
}

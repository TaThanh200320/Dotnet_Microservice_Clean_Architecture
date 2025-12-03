using System.Security.Claims;
using System.Security.Cryptography;
using Application.Interfaces.Services.Token;
using Contracts.Dtos.Responses;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;
using SharedKernel.Extensions;

namespace IdentityInfrastructure.Services.Token;

public class TokenFactoryService : ITokenFactoryService
{
    private readonly JwtSettings settings;
    private readonly RSA privateRsa;
    private readonly RSA publicRsa;

    public TokenFactoryService(IOptions<JwtSettings> jwtSettings)
    {
        settings = jwtSettings.Value;
        
        // Load private key for signing
        privateRsa = RSA.Create();
        var privateKeyPem = File.ReadAllText(settings.PrivateKeyPath!);
        privateRsa.ImportFromPem(privateKeyPem);
        
        // Load public key for verification
        publicRsa = RSA.Create();
        var publicKeyPem = File.ReadAllText(settings.PublicKeyPath!);
        publicRsa.ImportFromPem(publicKeyPem);
    }

    public DateTime AccessTokenExpiredTime => GetAccessTokenExpiredTime();

    public DateTime RefreshTokenExpiredTime => GetRefreshTokenExpiredTime();

    public string CreateToken(IEnumerable<Claim> claims, DateTime expirationTime)
    {
        return JwtBuilder
            .Create()
            .WithAlgorithm(new RS256Algorithm(publicRsa, privateRsa))
            .AddClaims(claims.Select(x => new KeyValuePair<string, object>(x.Type, x.Value)))
            .ExpirationTime(expirationTime)
            .WithValidationParameters(
                new ValidationParameters() { ValidateExpirationTime = true, TimeMargin = 0 }
            )
            .Encode();
    }

    public DecodeTokenResponse DecodeToken(string token)
    {
        var json = JwtBuilder
            .Create()
            .WithAlgorithm(new RS256Algorithm(publicRsa, privateRsa))
            .MustVerifySignature()
            .Decode(token);

        return SerializerExtension.Deserialize<DecodeTokenResponse>(json).Object!;
    }

    private DateTime GetAccessTokenExpiredTime() =>
        DateTime.UtcNow.AddMinutes(double.Parse(settings.ExpireTimeAccessTokenInMinute!));

    private DateTime GetRefreshTokenExpiredTime() =>
        DateTime.UtcNow.AddDays(double.Parse(settings.ExpireTimeRefreshTokenInDay!));
}

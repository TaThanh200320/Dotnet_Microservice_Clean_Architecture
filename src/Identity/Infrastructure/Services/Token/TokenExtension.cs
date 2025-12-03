using System.Security.Cryptography;
using Application.Interfaces.Services.Token;
using IdentityApplication.Common.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Common;

namespace IdentityInfrastructure.Services.Token;

public static class TokenExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(
            config.GetSection($"SecuritySettings:{nameof(JwtSettings)}")
        );

        var jwtSettings = config
            .GetSection($"SecuritySettings:{nameof(JwtSettings)}")
            .Get<JwtSettings>();

        // Load public key for token validation
        var rsa = RSA.Create();
        var publicKeyPem = File.ReadAllText(jwtSettings!.PublicKeyPath!);
        rsa.ImportFromPem(publicKeyPem);
        var rsaSecurityKey = new RsaSecurityKey(rsa);

        return services
            .AddSingleton<ITokenFactoryService, TokenFactoryService>()
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaSecurityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };

                bearer.IncludeErrorDetails = true;
                bearer.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        return TokenErrorExtension.UnauthorizedException(
                            context,
                            !context.Response.HasStarted
                                ? new UnauthorizedError(Message.UNAUTHORIZED)
                                : new UnauthorizedError(Message.TOKEN_EXPIRED)
                        );
                    },
                    OnForbidden = context =>
                        TokenErrorExtension.ForbiddenException(
                            context,
                            new ForbiddenError(Message.FORBIDDEN)
                        ),
                };
            })
            .Services;
    }
}

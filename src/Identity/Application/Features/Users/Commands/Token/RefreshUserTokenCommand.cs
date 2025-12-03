using Contracts.ApiWrapper;
using Mediator;

namespace IdentityApplication.Features.Users.Commands.Token;

public class RefreshUserTokenCommand : IRequest<Result<RefreshUserTokenResponse>>
{
    public string? RefreshToken { get; set; }
}

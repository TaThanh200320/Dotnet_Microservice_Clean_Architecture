using Contracts.ApiWrapper;
using IdentityApplication.Features.Common.Payloads.Users;
using IdentityDomain.Aggregates.Users.Enums;
using Mediator;

namespace IdentityApplication.Features.Users.Commands.Create;

public class CreateUserCommand : UserPayload, IRequest<Result<CreateUserResponse>>
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    public Gender? Gender { get; set; }

    public UserStatus Status { get; set; }

    public List<Ulid>? Roles { get; set; }

    public List<UserClaimPayload>? UserClaims { get; set; }
}

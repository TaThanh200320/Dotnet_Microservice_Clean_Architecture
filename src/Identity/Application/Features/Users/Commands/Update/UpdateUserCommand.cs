using Contracts.ApiWrapper;
using IdentityApplication.Features.Common.Payloads.Users;
using Mediator;

namespace IdentityApplication.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<Result<UpdateUserResponse>>
{
    public string UserId { get; set; } = string.Empty;

    public UserUpdateRequest UpdateData { get; set; } = null!;
}

public class UserUpdateRequest : UserPayload
{
    public List<Ulid>? Roles { get; set; }

    public List<UserClaimPayload>? UserClaims { get; set; }
}

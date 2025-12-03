using IdentityApplication.Features.Common.Projections.Users;

namespace IdentityApplication.Features.Users.Commands.Login;

public class LoginUserResponse : UserTokenProjection
{
    public UserProjection? User { get; set; }
}

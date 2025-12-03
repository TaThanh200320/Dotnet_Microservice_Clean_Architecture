using Application.Interfaces.Services;
using FluentValidation;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityApplication.Features.Common.Validators.Users;

namespace IdentityApplication.Features.Users.Commands.Profiles;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator(
        IUserManagerService userManagerService,
        IHttpContextAccessorService httpContextAccessorService,
        ICurrentUser currentUser
    )
    {
        Include(new UserValidator(userManagerService, httpContextAccessorService, currentUser));
    }
}

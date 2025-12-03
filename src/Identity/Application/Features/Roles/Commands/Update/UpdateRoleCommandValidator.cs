using Application.Interfaces.Services;
using FluentValidation;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityApplication.Features.Common.Validators.Roles;

namespace IdentityApplication.Features.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<RoleUpdateRequest>
{
    public UpdateRoleCommandValidator(
        IRoleManagerService roleManagerService,
        IHttpContextAccessorService httpContextAccessorService
    )
    {
        Include(new RoleValidator(roleManagerService, httpContextAccessorService));
    }
}

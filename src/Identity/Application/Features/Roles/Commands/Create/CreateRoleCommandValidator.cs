using Application.Interfaces.Services;
using FluentValidation;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityApplication.Features.Common.Validators.Roles;

namespace IdentityApplication.Features.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(
        IRoleManagerService roleManagerService,
        IHttpContextAccessorService httpContextAccessorService
    )
    {
        Include(new RoleValidator(roleManagerService, httpContextAccessorService));
    }
}

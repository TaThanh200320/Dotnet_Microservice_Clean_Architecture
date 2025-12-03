using Contracts.ApiWrapper;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityDomain.Aggregates.Roles;
using Mediator;

namespace IdentityApplication.Features.Roles.Commands.Create;

public class CreateRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<CreateRoleCommand, Result<CreateRoleResponse>>
{
    public async ValueTask<Result<CreateRoleResponse>> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role mappingRole = command.ToRole();
        Role role = await roleManagerService.CreateAsync(mappingRole);
        return Result<CreateRoleResponse>.Success(role.ToCreateRoleResponse());
    }
}

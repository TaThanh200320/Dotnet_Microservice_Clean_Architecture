using Application.Errors;
using Contracts.ApiWrapper;
using IdentityApplication.Common.Constants;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityApplication.Features.Common.Mapping.Roles;
using IdentityDomain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Roles.Commands.Update;

public class UpdateRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<UpdateRoleCommand, Result<UpdateRoleResponse>>
{
    public async ValueTask<Result<UpdateRoleResponse>> Handle(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role? role = await roleManagerService.FindByIdAsync(Ulid.Parse(command.RoleId));

        if (role == null)
        {
            return Result<UpdateRoleResponse>.Failure(
                new NotFoundError(
                    TitleMessage.RESOURCE_NOT_FOUND,
                    Messenger
                        .Create<Role>()
                        .Message(MessageType.Found)
                        .Negative()
                        .VietnameseTranslation(TranslatableMessage.VI_ROLE_NOT_FOUND)
                        .BuildMessage()
                )
            );
        }

        role.FromUpdateRole(command.UpdateData);

        List<RoleClaim> roleClaims = command.UpdateData.RoleClaims.ToListRoleClaim() ?? [];
        await roleManagerService.UpdateAsync(role, roleClaims);
        return Result<UpdateRoleResponse>.Success(role.ToUpdateRoleResponse());
    }
}

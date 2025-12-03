using Application.Errors;
using Contracts.ApiWrapper;
using IdentityApplication.Common.Constants;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityDomain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Roles.Commands.Delete;

public class DeleteRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<DeleteRoleCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role? role = await roleManagerService.FindByIdAsync(command.RoleId);

        if (role == null)
        {
            return Result<string>.Failure(
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

        await roleManagerService.DeleteAsync(role);

        return Result<string>.Success();
    }
}

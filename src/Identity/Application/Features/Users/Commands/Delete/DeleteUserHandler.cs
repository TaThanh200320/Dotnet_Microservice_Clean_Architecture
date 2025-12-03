using Application.Errors;
using Application.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using IdentityApplication.Common.Constants;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityDomain.Aggregates.Users;
using IdentityDomain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Users.Commands.Delete;

public class DeleteUserHandler(IUnitOfWork unitOfWork, IMediaUpdateService<User> mediaUpdateService)
    : IRequestHandler<DeleteUserCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdWithoutIncludeSpecification(command.UserId),
                cancellationToken
            );

        if (user == null)
        {
            return Result<string>.Failure(
                new NotFoundError(
                    TitleMessage.RESOURCE_NOT_FOUND,
                    Messenger
                        .Create<User>()
                        .Message(MessageType.Found)
                        .Negative()
                        .VietnameseTranslation(TranslatableMessage.VI_USER_NOT_FOUND)
                        .BuildMessage()
                )
            );
        }
        string? avatar = user.Avatar;
        await unitOfWork.Repository<User>().DeleteAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        await mediaUpdateService.DeleteAvatarAsync(avatar);
        return Result<string>.Success();
    }
}

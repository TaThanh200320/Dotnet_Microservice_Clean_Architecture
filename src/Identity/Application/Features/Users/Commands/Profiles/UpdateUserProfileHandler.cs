using Application.Errors;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using IdentityApplication.Common.Constants;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityDomain.Aggregates.Users;
using IdentityDomain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Users.Commands.Profiles;

public class UpdateUserProfileHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMediaUpdateService<User> avatarUpdate
) : IRequestHandler<UpdateUserProfileCommand, Result<UpdateUserProfileResponse>>
{
    public async ValueTask<Result<UpdateUserProfileResponse>> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdWithoutIncludeSpecification(currentUser.Id ?? Ulid.Empty),
                cancellationToken
            );

        if (user == null)
        {
            return Result<UpdateUserProfileResponse>.Failure(
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

        IFormFile? avatar = command.Avatar;
        string? oldAvatar = user.Avatar;

        user.MapFromUpdateUserProfileCommand(command);

        string? key = avatarUpdate.GetKey(avatar);
        user.Avatar = await avatarUpdate.UploadAvatarAsync(avatar, key);

        try
        {
            await unitOfWork.Repository<User>().UpdateAsync(user);
            await unitOfWork.SaveAsync(cancellationToken);
            await avatarUpdate.DeleteAvatarAsync(oldAvatar);
        }
        catch (Exception)
        {
            await avatarUpdate.DeleteAvatarAsync(user.Avatar);
            throw;
        }

        UpdateUserProfileResponse? response = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdSpecification(user.Id),
                x => x.ToUpdateUserProfileResponse(),
                cancellationToken
            );

        return Result<UpdateUserProfileResponse>.Success(response!);
    }
}

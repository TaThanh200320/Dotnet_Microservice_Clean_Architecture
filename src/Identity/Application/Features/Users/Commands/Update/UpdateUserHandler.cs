using Application.Errors;
using Application.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using IdentityApplication.Common.Constants;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityDomain.Aggregates.Users;
using IdentityDomain.Aggregates.Users.Enums;
using IdentityDomain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Users.Commands.Update;

public class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMediaUpdateService<User> mediaUpdateService,
    IUserManagerService userManagerService
) : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    public async ValueTask<Result<UpdateUserResponse>> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await GetUserAsync(Ulid.Parse(command.UserId), cancellationToken);
        if (user == null)
        {
            return Result<UpdateUserResponse>.Failure(
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

        UserUpdateRequest updateData = command.UpdateData;

        IFormFile? avatar = updateData.Avatar;
        string? oldAvatar = user.Avatar;

        user.FromUpdateUser(updateData);

        string? key = mediaUpdateService.GetKey(avatar);
        user.Avatar = await mediaUpdateService.UploadAvatarAsync(avatar, key);

        //* trigger event to update default claims -  that's information of user
        user.UpdateDefaultUserClaims();

        try
        {
            _ = await unitOfWork.BeginTransactionAsync(cancellationToken);

            await unitOfWork.Repository<User>().UpdateAsync(user);
            await unitOfWork.SaveAsync(cancellationToken);

            //* update custom claims of user like permissions ...
            List<UserClaim> customUserClaims =
                updateData.UserClaims?.ToListUserClaim(UserClaimType.Custom, user.Id) ?? [];
            await userManagerService.UpdateAsync(user, updateData.Roles!, customUserClaims);

            await unitOfWork.CommitAsync(cancellationToken);

            await mediaUpdateService.DeleteAvatarAsync(oldAvatar);
            User? userResponse = await GetUserAsync(user.Id, cancellationToken);
            return Result<UpdateUserResponse>.Success(userResponse!.ToUpdateUserResponse());
        }
        catch (Exception)
        {
            await mediaUpdateService.DeleteAvatarAsync(user.Avatar);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<User?> GetUserAsync(Ulid id, CancellationToken cancellationToken)
    {
        return await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(new GetUserByIdSpecification(id), cancellationToken);
    }
}

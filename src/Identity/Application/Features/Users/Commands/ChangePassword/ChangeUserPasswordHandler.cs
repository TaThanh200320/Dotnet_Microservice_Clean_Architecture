using Application.Errors;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using IdentityApplication.Common.Constants;
using IdentityDomain.Aggregates.Users;
using IdentityDomain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;
using static BCrypt.Net.BCrypt;

namespace IdentityApplication.Features.Users.Commands.ChangePassword;

public class ChangeUserPasswordHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    : IRequestHandler<ChangeUserPasswordCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        Ulid? userId = currentUser.Id;
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdWithoutIncludeSpecification(userId ?? Ulid.Empty),
                cancellationToken
            );

        if (user == null)
        {
            return Result<string>.Failure(
                new NotFoundError(
                    "The TitleMessage.RESOURCE_NOT_FOUND",
                    Messenger
                        .Create<User>()
                        .Message(MessageType.Found)
                        .Negative()
                        .VietnameseTranslation(TranslatableMessage.VI_USER_NOT_FOUND)
                        .Build()
                )
            );
        }

        if (!Verify(request.OldPassword, user.Password))
        {
            return Result<string>.Failure(
                new BadRequestError(
                    "Error has occured with password",
                    Messenger
                        .Create<ChangeUserPasswordCommand>(nameof(User))
                        .Property(x => x.OldPassword!)
                        .Message(MessageType.Correct)
                        .Negative()
                        .Build()
                )
            );
        }

        user.SetPassword(HashPassword(request.NewPassword));

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return Result<string>.Success();
    }
}

using Application.Interfaces.Services;
using FluentValidation;
using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityApplication.Features.Common.Validators.Users;
using IdentityDomain.Aggregates.Users;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UserUpdateRequest>
{
    public UpdateUserCommandValidator(
        IUserManagerService userManagerService,
        IHttpContextAccessorService httpContextAccessorService,
        ICurrentUser currentUser
    )
    {
        Include(new UserValidator(userManagerService, httpContextAccessorService,currentUser)!);

        RuleFor(x => x.Roles)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<UserUpdateRequest>(nameof(User))
                    .Property(x => x.Roles!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        When(
            x => x.UserClaims != null,
            () =>
            {
                RuleForEach(x => x!.UserClaims).SetValidator(new UserClaimValidator());
            }
        );
    }
}

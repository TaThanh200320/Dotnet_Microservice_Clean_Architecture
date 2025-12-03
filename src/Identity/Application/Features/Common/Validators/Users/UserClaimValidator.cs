using FluentValidation;
using IdentityApplication.Features.Common.Payloads.Users;
using IdentityDomain.Aggregates.Users;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Common.Validators.Users;

public class UserClaimValidator : AbstractValidator<UserClaimPayload>
{
    public UserClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<UserClaim>(nameof(User.UserClaims))
                    .Property(x => x.ClaimType!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.ClaimValue)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<UserClaim>(nameof(User.UserClaims))
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );
    }
}

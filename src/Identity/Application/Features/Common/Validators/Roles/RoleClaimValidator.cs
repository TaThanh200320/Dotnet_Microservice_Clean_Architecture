using FluentValidation;
using IdentityApplication.Features.Common.Payloads.Roles;
using IdentityDomain.Aggregates.Roles;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Features.Common.Validators.Roles;

public class RoleClaimValidator : AbstractValidator<RoleClaimPayload>
{
    public RoleClaimValidator()
    {
        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<RoleClaim>(nameof(Role.RoleClaims))
                    .Property(x => x.ClaimType!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.ClaimValue)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<RoleClaim>(nameof(Role.RoleClaims))
                    .Property(x => x.ClaimValue!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );
    }
}

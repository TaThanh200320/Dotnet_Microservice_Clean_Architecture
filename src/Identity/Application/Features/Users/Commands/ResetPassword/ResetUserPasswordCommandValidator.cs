using FluentValidation;
using IdentityDomain.Aggregates.Users;
using SharedKernel.Common.Messages;
using Application.Extensions;

namespace IdentityApplication.Features.Users.Commands.ResetPassword;

public class UpdateUserPasswordValidator : AbstractValidator<UpdateUserPassword>
{
    public UpdateUserPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<UserResetPassword>()
                    .Property(x => x.Token)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            );

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithState(x =>
                Messenger
                    .Create<User>()
                    .Property(x => x.Password!)
                    .Message(MessageType.Null)
                    .Negative()
                    .Build()
            )
            .Must(x => x!.IsValidPassword())
            .WithState(x =>
                Messenger
                    .Create<User>(nameof(User))
                    .Property(x => x.Password)
                    .Message(MessageType.Strong)
                    .Negative()
                    .Build()
            );
    }
}

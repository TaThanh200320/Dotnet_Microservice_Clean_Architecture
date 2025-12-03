using FluentValidation;

namespace IdentityApplication.Features.Users.Commands.Login;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator() { }
}

using Contracts.ApiWrapper;
using Mediator;

namespace IdentityApplication.Features.Users.Commands.RequestResetPassword;

public record RequestResetUserPasswordCommand(string Email) : IRequest<Result<string>>;

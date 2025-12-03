using Contracts.ApiWrapper;
using Mediator;

namespace IdentityApplication.Features.Users.Commands.Delete;

public record DeleteUserCommand(Ulid UserId) : IRequest<Result<string>>;

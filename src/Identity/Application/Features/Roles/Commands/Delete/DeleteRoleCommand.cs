using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApplication.Features.Roles.Commands.Delete;

public record DeleteRoleCommand([FromRoute] Ulid RoleId) : IRequest<Result<string>>;

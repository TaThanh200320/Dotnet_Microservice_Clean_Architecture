using Contracts.ApiWrapper;
using IdentityApplication.Features.Common.Payloads.Roles;
using Mediator;

namespace IdentityApplication.Features.Roles.Commands.Create;

public class CreateRoleCommand : RolePayload, IRequest<Result<CreateRoleResponse>>;

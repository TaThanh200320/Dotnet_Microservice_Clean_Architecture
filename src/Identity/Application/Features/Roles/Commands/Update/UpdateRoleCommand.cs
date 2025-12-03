using Contracts.ApiWrapper;
using IdentityApplication.Features.Common.Payloads.Roles;
using Mediator;

namespace IdentityApplication.Features.Roles.Commands.Update;

public class UpdateRoleCommand : IRequest<Result<UpdateRoleResponse>>
{
    public string RoleId { get; set; } = string.Empty;

    public RoleUpdateRequest UpdateData { get; set; } = null!;
}

public class RoleUpdateRequest : RolePayload;

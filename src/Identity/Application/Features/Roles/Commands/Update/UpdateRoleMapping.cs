using IdentityDomain.Aggregates.Roles;
using SharedKernel.Extensions;

namespace IdentityApplication.Features.Roles.Commands.Update;

public static class UpdateRoleMapping
{
    public static Role FromUpdateRole(this Role role, RoleUpdateRequest RoleUpdateRequest)
    {
        role.Name = RoleUpdateRequest.Name.ToScreamingSnakeCase();
        role.Description = RoleUpdateRequest.Description;
        return role;
    }

    public static UpdateRoleResponse ToUpdateRoleResponse(this Role role)
    {
        UpdateRoleResponse roleResponse = new();
        roleResponse.MappingFrom(role);
        return roleResponse;
    }
}

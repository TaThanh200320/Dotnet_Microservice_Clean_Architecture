using CaseConverter;
using IdentityApplication.Features.Common.Mapping.Roles;
using IdentityDomain.Aggregates.Roles;

namespace IdentityApplication.Features.Roles.Commands.Create;

public static class CreateRoleMapping
{
    public static Role ToRole(this CreateRoleCommand roleCommand) =>
        new()
        {
            Name = roleCommand.Name.ToSnakeCase().ToUpper(),
            Description = roleCommand.Description,
            RoleClaims = roleCommand.RoleClaims?.ToListRoleClaim(),
        };

    public static CreateRoleResponse ToCreateRoleResponse(this Role role)
    {
        CreateRoleResponse roleResponse = new();
        roleResponse.MappingFrom(role);
        return roleResponse;
    }
}

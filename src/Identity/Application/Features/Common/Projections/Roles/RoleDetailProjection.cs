using IdentityApplication.Features.Common.Mapping.Roles;
using IdentityDomain.Aggregates.Roles;

namespace IdentityApplication.Features.Common.Projections.Roles;

public class RoleDetailProjection : RoleProjection
{
    public ICollection<RoleClaimDetailProjection>? RoleClaims { get; set; }

    public override void MappingFrom(Role role)
    {
        base.MappingFrom(role);
        RoleClaims = role.RoleClaims?.ToListRoleClaimDetailProjection();
    }
}

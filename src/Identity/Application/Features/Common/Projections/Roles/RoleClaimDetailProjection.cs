
using SharedKernel.Models;

namespace IdentityApplication.Features.Common.Projections.Roles;

public class RoleClaimDetailProjection : DefaultBaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}

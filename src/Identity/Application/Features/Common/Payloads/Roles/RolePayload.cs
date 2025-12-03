
namespace IdentityApplication.Features.Common.Payloads.Roles;

public class RolePayload
{
    public string? Description { get; set; }

    public required string? Name { get; set; }

    public List<RoleClaimPayload>? RoleClaims { get; set; }
}

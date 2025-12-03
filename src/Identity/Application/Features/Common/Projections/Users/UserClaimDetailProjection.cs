using System.Text.Json.Serialization;
using IdentityDomain.Aggregates.Users.Enums;
using SharedKernel.Models;

namespace IdentityApplication.Features.Common.Projections.Users;

public class UserClaimDetailProjection : DefaultBaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public UserClaimType Type { get; set; }
}

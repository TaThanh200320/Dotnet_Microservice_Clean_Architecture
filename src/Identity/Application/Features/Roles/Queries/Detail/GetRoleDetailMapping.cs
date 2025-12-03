using IdentityDomain.Aggregates.Roles;

namespace IdentityApplication.Features.Roles.Queries.Detail;

// public class GetRoleDetailMapping : Profile
// {
//     public GetRoleDetailMapping()
//     {
//         CreateMap<Role, RoleDetailResponse>();
//     }
// }

public static class GetRoleDetailMapping
{
    public static RoleDetailResponse ToRoleDetailResponse(this Role role)
    {
        RoleDetailResponse detailResponse = new();
        detailResponse.MappingFrom(role);

        return detailResponse;
    }
}

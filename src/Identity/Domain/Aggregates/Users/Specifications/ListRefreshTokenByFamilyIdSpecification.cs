using Specification;
using Specification.Builders;

namespace IdentityDomain.Aggregates.Users.Specifications;

public class ListRefreshTokenByFamilyIdSpecification : Specification<UserToken>
{
    public ListRefreshTokenByFamilyIdSpecification(string familyId, Ulid userId)
    {
        Query.Where(x => x.FamilyId == familyId && x.UserId == userId).Include(x => x.User);
    }
}

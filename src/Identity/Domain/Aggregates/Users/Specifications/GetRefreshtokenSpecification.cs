using Specification;
using Specification.Builders;

namespace IdentityDomain.Aggregates.Users.Specifications;

public class GetRefreshtokenSpecification : Specification<UserToken>
{
    public GetRefreshtokenSpecification(string token, Ulid userId)
    {
        Query.Where(x => x.UserId == userId && x.RefreshToken == token).Include(x => x.User);
    }
}

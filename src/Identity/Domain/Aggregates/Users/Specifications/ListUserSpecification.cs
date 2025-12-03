using Specification;
using Specification.Builders;

namespace IdentityDomain.Aggregates.Users.Specifications;

public class ListUserSpecification : Specification<User>
{
    public ListUserSpecification()
    {
        string key = GetUniqueCachedKey();
        Query.EnableCache(key);
    }
}

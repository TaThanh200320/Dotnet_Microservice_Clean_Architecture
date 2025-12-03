using Specification;
using Specification.Builders;

namespace IdentityDomain.Aggregates.Users.Specifications;

public class GetUserByIdWithoutIncludeSpecification : Specification<User>
{
    public GetUserByIdWithoutIncludeSpecification(Ulid id)
    {
        Query.Where(x => x.Id == id).AsNoTracking();
    }
}

using Specification;
using Specification.Builders;

namespace IdentityDomain.Aggregates.Users.Specifications;

public class GetUserByIdIncludeResetPassword : Specification<User>
{
    public GetUserByIdIncludeResetPassword(Ulid id)
    {
        Query.Where(x => x.Id == id).Include(x => x.UserResetPasswords).AsNoTracking();
    }
}

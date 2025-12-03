using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace IdentityApplication.Features.Roles.Queries.List;

public class ListRoleQuery() : QueryParamRequest, IRequest<Result<IEnumerable<ListRoleResponse>>>
{
    public static ValueTask<ListRoleQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListRoleQuery>(context));
    }
}

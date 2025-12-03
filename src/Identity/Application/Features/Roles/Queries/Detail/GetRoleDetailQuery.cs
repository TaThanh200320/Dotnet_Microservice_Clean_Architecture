using Contracts.ApiWrapper;
using Mediator;

namespace IdentityApplication.Features.Roles.Queries.Detail;

public record GetRoleDetailQuery(Ulid Id) : IRequest<Result<RoleDetailResponse>>;

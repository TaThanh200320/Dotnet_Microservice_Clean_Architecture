using Contracts.ApiWrapper;
using Mediator;

namespace IdentityApplication.Features.Users.Queries.Detail;

public record GetUserDetailQuery(Ulid UserId) : IRequest<Result<GetUserDetailResponse>>;

using Contracts.ApiWrapper;
using Mediator;

namespace IdentityApplication.Features.Users.Queries.Profiles;

public class GetUserProfileQuery() : IRequest<Result<GetUserProfileResponse>>;

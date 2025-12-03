using Contracts.ApiWrapper;
using IdentityApplication.Features.Common.Payloads.Users;
using Mediator;

namespace IdentityApplication.Features.Users.Commands.Profiles;

public class UpdateUserProfileCommand : UserPayload, IRequest<Result<UpdateUserProfileResponse>>;

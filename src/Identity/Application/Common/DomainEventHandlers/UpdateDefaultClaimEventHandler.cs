using IdentityApplication.Common.Interfaces.Services.Identity;
using IdentityDomain.Aggregates.Users;
using IdentityDomain.Aggregates.Users.Events;
using Mediator;

namespace IdentityApplication.Common.DomainEventHandlers;

public class UpdateDefaultClaimEventHandler(IUserManagerService userManagerService)
    : INotificationHandler<UpdateDefaultUserClaimEvent>
{
    public async ValueTask Handle(
        UpdateDefaultUserClaimEvent notification,
        CancellationToken cancellationToken
    )
    {
        User user = notification.User!;
        IReadOnlyCollection<UserClaim> defaultUserClaims = user.DefaultUserClaimsToUpdates;
        await userManagerService.UpdateDefaultUserClaimsAsync(defaultUserClaims);
    }
}

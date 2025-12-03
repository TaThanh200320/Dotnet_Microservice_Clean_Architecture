using Mediator;

namespace IdentityDomain.Aggregates.Users.Events;

public class UpdateDefaultUserClaimEvent : INotification
{
    public User? User { get; set; }
}

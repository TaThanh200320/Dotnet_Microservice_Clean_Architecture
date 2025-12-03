using IdentityApi.Common.Routers;
using Microsoft.AspNetCore.Routing;

namespace IdentityApi.Common.EndpointConfigurations;

public interface IEndpoint
{
    public EndpointVersion Version { get; }

    public void MapEndpoint(IEndpointRouteBuilder app);
}

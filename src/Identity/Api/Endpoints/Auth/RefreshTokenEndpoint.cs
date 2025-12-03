using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Commands.Token;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.Auth;

public class RefreshTokenEndpoint() : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.AuthRoute.RefreshToken, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Refresh Access Token 🔄 🔐",
                Description = "obtains a new pair of token by providing a valid refresh token.",
                Tags = [new OpenApiTag() { Name = Router.AuthRoute.Tags }],
            });
    }

    private async Task<
        Results<Ok<ApiResponse<RefreshUserTokenResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromBody] RefreshUserTokenCommand request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}

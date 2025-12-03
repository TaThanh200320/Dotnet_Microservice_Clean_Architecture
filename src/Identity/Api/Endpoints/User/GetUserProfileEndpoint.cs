using Api.Results;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Cache;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Queries.Profiles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.User;

public class GetUserProfileEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.UserRoute.Profile, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get current user's profile 🧑‍💼",
                Description = "Returns user profile if found",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .RequireAuth();
    }

    private async Task<
        Results<Ok<ApiResponse<GetUserProfileResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromServices] ISender sender,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IMemoryCacheService cacheService,
        CancellationToken cancellationToken = default
    )
    {
        Ulid? userId = currentUser.Id;
        var result = await cacheService.GetOrSetAsync(
            $"{nameof(GetUserProfileEndpoint)}:{userId}",
            () => sender.Send(new GetUserProfileQuery(), cancellationToken).AsTask(),
            new CacheOptions()
            {
                ExpirationType = CacheExpirationType.Sliding,
                Expiration = TimeSpan.FromMinutes(15),
            }
        );
        return result!.ToResult();
    }
}

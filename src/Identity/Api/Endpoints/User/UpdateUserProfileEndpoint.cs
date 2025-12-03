using Api.EndpointConfigurations;
using Api.Results;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Cache;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Commands.Profiles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.User;

public class UpdateUserProfileEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.UserRoute.Profile, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update user profile 🛠️ 👨 📋",
                Description = "Updates profile information for the currently authenticated user.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<UpdateUserProfileCommand>()
            .RequireAuth()
            .DisableAntiforgery();
    }

    private async Task<
        Results<Ok<ApiResponse<UpdateUserProfileResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromForm] UpdateUserProfileCommand request,
        [FromServices] ISender sender,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IMemoryCacheService cacheService,
        CancellationToken cancellationToken = default
    )
    {
        Ulid? userId = currentUser.Id;
        string key = $"{nameof(GetUserProfileEndpoint)}:{userId}";
        bool isExisted = cacheService.HasKey(key);
        if (isExisted)
        {
            cacheService.Remove(key);
        }
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}

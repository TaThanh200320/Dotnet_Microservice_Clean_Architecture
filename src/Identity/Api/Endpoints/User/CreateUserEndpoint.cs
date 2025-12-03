using Api.EndpointConfigurations;
using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Commands.Create;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.User;

public class CreateUserEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.UserRoute.Users, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Create user 🧑",
                Description = "Creates a new user and returns the created user details.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<CreateUserCommand>()
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Create, PermissionResource.User)
            )
            .DisableAntiforgery();
    }

    private async Task<
        Results<CreatedAtRoute<ApiResponse<CreateUserResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromForm] CreateUserCommand request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToCreatedResult(result.Value!.Id, Router.UserRoute.GetRouteName);
    }
}

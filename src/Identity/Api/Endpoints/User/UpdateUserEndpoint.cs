using Api.EndpointConfigurations;
using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Commands.Update;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.User;

public class UpdateUserEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.UserRoute.GetUpdateDelete, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = " Update user ✏️ 🧑‍💻",
                Description = "Updates the information of an existing user identified by their ID.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<UserUpdateRequest>()
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Update, PermissionResource.User)
            )
            .DisableAntiforgery();
    }

    private async Task<Results<Ok<ApiResponse<UpdateUserResponse>>, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromForm] UserUpdateRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new UpdateUserCommand() { UserId = id, UpdateData = request };
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}

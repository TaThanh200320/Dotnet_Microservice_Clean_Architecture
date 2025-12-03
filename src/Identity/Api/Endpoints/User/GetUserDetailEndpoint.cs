using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Queries.Detail;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.User;

public class GetUserDetailEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.UserRoute.GetUpdateDelete, HandleAsync)
            .WithName(Router.UserRoute.GetRouteName)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get user by ID 🧾",
                Description = "Retrieves detailed information of a user based on their unique ID.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Detail, PermissionResource.User)
            );
    }

    private async Task<
        Results<Ok<ApiResponse<GetUserDetailResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new GetUserDetailQuery(Ulid.Parse(id));
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}

using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Roles.Queries.Detail;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.Roles;

public class GetRoleDetailEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.RoleRoute.GetUpdateDelete, HandleAsync)
            .WithName(Router.RoleRoute.GetRouteName)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get role details 🔎",
                Description =
                    "Retrieves detailed information about a specific role, including its name and associated claims/permissions. Use this to review or audit the role’s configurations.",
                Tags = [new OpenApiTag() { Name = Router.RoleRoute.Tags }],
            })
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Detail, PermissionResource.Role)
            );
    }

    private async Task<Results<Ok<ApiResponse<RoleDetailResponse>>, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new GetRoleDetailQuery(Ulid.Parse(id));
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}

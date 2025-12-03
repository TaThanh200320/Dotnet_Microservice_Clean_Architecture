using Api.EndpointConfigurations;
using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Roles.Commands.Create;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.Roles;

public class CreateRoleEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Router.RoleRoute.Roles, HandleAsync)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Create role 👮",
                Description =
                    "Creates a new role with optional claims like permissions, etc. This endpoint can be used to define the authorization boundaries within your application. Provide a list of claims to associate them with the newly created role.",
                Tags = [new OpenApiTag() { Name = Router.RoleRoute.Tags }],
            })
            .WithRequestValidation<CreateRoleCommand>()
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Create, PermissionResource.Role)
            );
    }

    private async Task<
        Results<CreatedAtRoute<ApiResponse<CreateRoleResponse>>, ProblemHttpResult>
    > HandleAsync(
        [FromBody] CreateRoleCommand request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToCreatedResult(result.Value!.Id, Router.RoleRoute.GetRouteName);
    }
}

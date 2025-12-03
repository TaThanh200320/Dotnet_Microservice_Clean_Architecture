using Api.EndpointConfigurations;
using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Roles.Commands.Update;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.Roles;

public class UpdateRoleEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.RoleRoute.GetUpdateDelete, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update role 📝",
                Description =
                    "Updates an existing role's information. You can modify the name and add or remove claims/permissions. This endpoint helps ensure your authorization model stays current with your users' needs.",
                Tags = [new OpenApiTag() { Name = Router.RoleRoute.Tags }],
            })
            .WithRequestValidation<RoleUpdateRequest>()
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Update, PermissionResource.Role)
            );
    }

    private async Task<Results<Ok<ApiResponse<UpdateRoleResponse>>, ProblemHttpResult>> HandleAsync(
        [FromRoute] string id,
        [FromBody] RoleUpdateRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new UpdateRoleCommand() { RoleId = id.ToString(), UpdateData = request };
        var result = await sender.Send(command, cancellationToken);
        return result.ToResult();
    }
}

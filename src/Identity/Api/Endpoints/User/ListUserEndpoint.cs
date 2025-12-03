using Api.Documents;
using Api.Results;
using Contracts.ApiWrapper;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Queries.List;
using IdentityInfrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using SharedKernel.Models;

namespace IdentityApi.Endpoints.User;

public class ListUserEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.UserRoute.Users, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get list of user 📄",
                Description = "Retrieves a list of all registered users in the system.",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
                Parameters = operation.AddDocs(),
            })
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.List, PermissionResource.User)
            );
    }

    private async Task<
        Results<Ok<ApiResponse<PaginationResponse<ListUserResponse>>>, ProblemHttpResult>
    > HandleAsync(
        ListUserQuery request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}

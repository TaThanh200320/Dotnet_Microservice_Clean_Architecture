using Api.Results;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Commands.ChangePassword;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.Auth;

public class ChangePasswordEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.AuthRoute.ChangePassword, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Change user password 🔑",
                Description =
                    "Allows an authenticated user to change their current password by providing the old and new password.",
                Tags = [new OpenApiTag() { Name = Router.AuthRoute.Tags }],
            })
            .RequireAuth();
    }

    private async Task<Results<NoContent, ProblemHttpResult>> HandleAsync(
        [FromBody] ChangeUserPasswordCommand request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToNoContentResult();
    }
}


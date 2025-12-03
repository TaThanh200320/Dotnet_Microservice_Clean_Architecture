using Api.EndpointConfigurations;
using Api.Results;
using IdentityApi.Common.EndpointConfigurations;
using IdentityApi.Common.Routers;
using IdentityApplication.Features.Users.Commands.ResetPassword;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace IdentityApi.Endpoints.Auth;

public class ResetPasswordEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Router.AuthRoute.ResetPassword, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Reset user password 🔄 🔑",
                Description =
                    "Resets a user's password using a valid token from a password reset request.",
                Tags = [new OpenApiTag() { Name = Router.AuthRoute.Tags }],
            }).WithRequestValidation<UpdateUserPassword>();
    }

    private async Task<Results<NoContent, ProblemHttpResult>> HandleAsync(
        [FromBody] UpdateUserPassword request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(
            new ResetUserPasswordCommand { UpdateUserPassword = request },
            cancellationToken
        );
        return result.ToNoContentResult();
    }
}

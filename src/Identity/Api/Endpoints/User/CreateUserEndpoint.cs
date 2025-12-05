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
                Description = @"Creates a new user with multipart/form-data.

                **Required Fields:**
                - `firstName`: User's first name
                - `lastName`: User's last name  
                - `username`: Unique username (alphanumeric, underscore, dot)
                - `password`: Strong password (min 8 chars, uppercase, lowercase, number, special char)
                - `email`: Valid email address
                - `phoneNumber`: Phone number with country code (e.g., +84979491460)
                - `gender`: 1 (Male), 2 (Female), 3 (Other)
                - `status`: 1 (Active), 2 (Inactive), 3 (Banned)
                - `roles`: Role ID (ULID format, e.g., 01JB19HK30BGYJBZGNETQY8905 for MANAGER)

                **Optional Fields:**
                - `dayOfBirth`: ISO 8601 date format
                - `avatar`: User avatar image file (IFormFile)
                - `userClaims`: Custom user claims array

                **Example Role IDs:**
                - ADMIN: 01J79JQZRWAKCTCQV64VYKMZ56
                - MANAGER: 01JB19HK30BGYJBZGNETQY8905",
                Tags = [new OpenApiTag() { Name = Router.UserRoute.Tags }],
            })
            .WithRequestValidation<CreateUserCommand>()
            .RequireAuth(
                permissions: Permission.Generate(PermissionAction.Create, PermissionResource.User)
            )
            .DisableAntiforgery()
            .Accepts<CreateUserCommand>("multipart/form-data");
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

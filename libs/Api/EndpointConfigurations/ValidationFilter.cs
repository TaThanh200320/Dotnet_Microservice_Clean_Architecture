using Application.Errors;
using Contracts.ApiWrapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.EndpointConfigurations;

public class ValidationFilter<TRequest>(
    IValidator<TRequest> validator,
    ILogger<ValidationFilter<TRequest>> logger
) : IEndpointFilter
    where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        TRequest? request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        
        // If request is null and content type is form-data, proceed to let model binding happen
        if (request is null)
        {
            var contentType = context.HttpContext.Request.ContentType;
            
            // For multipart/form-data, model binding happens during handler execution
            // So skip validation here and let it be handled by the validator behavior in MediatR
            if (contentType != null && contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogDebug(
                    "ValidationFilter: Skipping validation for {RequestType} because content-type is multipart/form-data",
                    typeof(TRequest).Name
                );
                return await next(context);
            }
            
            // Log for debugging
            logger.LogWarning(
                "ValidationFilter: Could not find {RequestType} in arguments. Arguments count: {Count}, Types: {Types}",
                typeof(TRequest).Name,
                context.Arguments.Count,
                string.Join(", ", context.Arguments.Select(a => a?.GetType().Name ?? "null"))
            );
            
            return TypedResults.Problem(
                new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = $"Request body of type {typeof(TRequest).Name} is required but was not provided.",
                }
            );
        }
        
        var validationResult = await validator.ValidateAsync(
            request,
            context.HttpContext.RequestAborted
        );

        if (!validationResult.IsValid)
        {
            Result<TRequest> result = Result<TRequest>.Failure(
                new ValidationError(validationResult.Errors)
            );
            ErrorDetails failure = result.Error!;

            return TypedResults.Problem(
                new ProblemDetails()
                {
                    Status = failure.Status,
                    Title = failure.Title,
                    Type = failure.Type,
                    Extensions = new Dictionary<string, object?>()
                    {
                        { "invalidParams", failure.InvalidParams },
                    },
                }
            );
        }

        return await next(context);
    }
}

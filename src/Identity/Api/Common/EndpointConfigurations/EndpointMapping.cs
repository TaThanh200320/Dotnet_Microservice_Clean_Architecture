using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Contracts.Constants;
using IdentityApi.Common.Routers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityApi.Common.EndpointConfigurations;

public static class EndpointMapping
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo<IEndpoint>())
                .As<IEndpoint>()
                .WithSingletonLifetime()
        );
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        EndpointVersion apiVersion
    )
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion((int)apiVersion))
            .ReportApiVersions()
            .Build();

        List<IEndpoint> endpoints =
        [
            .. app
                .Services.GetRequiredService<IEnumerable<IEndpoint>>()
                .Where(endpoint => endpoint.Version == apiVersion),
        ];

        RouteGroupBuilder routeGroupBuilder = app.MapGroup(
                $"/{RoutePath.prefix}" + "v{version:apiVersion}/"
            )
            .WithApiVersionSet(apiVersionSet);

        endpoints.ForEach(endpoint => endpoint.MapEndpoint(routeGroupBuilder));
        return app;
    }
}

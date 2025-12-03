using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions;

public static class HealthCheckExtension
{
    public static void AddDatabaseHealthCheck(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        DatabaseSettings settings =
            configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>() ?? new();

        services.AddNpgsqlDataSource(settings.DatabaseConnection!);
        services.AddHealthChecks().AddNpgSql();
    }
}

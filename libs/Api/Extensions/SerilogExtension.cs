using Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Api.Extensions;

public static class SerilogExtension
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(
            builder.Configuration
        );

        SerilogSettings serilogSettings =
            builder.Configuration.GetSection(nameof(SerilogSettings)).Get<SerilogSettings>()
            ?? new();

        if (serilogSettings!.IsDistributeLog)
        {
            loggerConfiguration.WriteTo.Seq(serilogSettings.SeqUrl!);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
        builder.Services.AddSingleton(Log.Logger);
    }
}

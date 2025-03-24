using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyDigimal.Api.Azure.Middleware;
using MyDigimal.Api.Azure.Models;
using MyDigimal.Data;

namespace MyDigimal.Api.Azure;

public static class Register
{
    /// <summary>
    /// Adds dependencies to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add services to.</param>
    public static void InitializeApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(
            configuration.GetSection("AppSettings"));

    }

    public static void RegisterConfiguration(this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionLoggingMiddleware>();
    }

    public static void RegisterCoreServices(this IServiceCollection services, Action? httpClientOverrideFunc = null)
    {
        if (httpClientOverrideFunc != null)
        {
            httpClientOverrideFunc();
        }
        else
        {
            services.AddHttpClient();
        }
    }


    public static void RegisterLogging(this ILoggingBuilder builder)
    {
        builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
#if DEBUG
        builder.SetMinimumLevel(LogLevel.Trace);
        builder.AddConsole();
#endif
#if !DEBUG
        builder.SetMinimumLevel(LogLevel.Information);        
        builder.AddApplicationInsights();
#endif
    }
}
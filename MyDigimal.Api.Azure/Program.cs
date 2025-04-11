using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyDigimal.Api.Azure;
using MyDigimal.Common;
using MyDigimal.Core;
using MyDigimal.Data;

var hostBuilder = Host.CreateDefaultBuilder(args);

IConfiguration configuration = null;

hostBuilder
    .ConfigureAppConfiguration(config =>
    {
        configuration = config.Build();
    })
    .ConfigureFunctionsWorkerDefaults(x =>
    {
        x.RegisterConfiguration();
        
    })
    .ConfigureServices((context, services) =>
    {
        var env = context.HostingEnvironment;
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        services.AddCors(options =>
        {
            if (env.IsDevelopment())
            {
                options.AddDefaultPolicy(builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            }
            else
            {
                options.AddDefaultPolicy(builder =>
                    builder
                        .WithOrigins(
                            "https://mydigimal.com",
                            "https://test.mydigimal.com",
                            "https://dev.mydigimal.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            }
        });

        services.InitializeApiDependencies(configuration);
        services.InitializeDataDependencies(configuration);
        services.InitializeCommonDependencies(configuration);
        services.InitializeCoreDependencies(configuration);
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.RegisterCoreServices();
    })
    .ConfigureLogging((_, logging) => logging.RegisterLogging());

hostBuilder.Build().Run();
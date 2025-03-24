using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDigimal.Core.AccountPlans;
using MyDigimal.Core.Authentication.Models;
using MyDigimal.Core.Caching;
using MyDigimal.Core.Handlers;
using MyDigimal.Core.Handlers.Processors;
using MyDigimal.Core.LogEntries;
using MyDigimal.Core.Providers;
using MyDigimal.Core.Schemas;
using MyDigimal.Core.Utilities;

namespace MyDigimal.Core;

public static class Register
{
    /// <summary>
    /// Adds dependencies to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">See <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" />.</param>
    public static void InitializeCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICaching, MemoryCaching>();
        services.AddTransient<ILogSchemaFactory, LogSchemaFactory>();
        services.AddTransient<ILogEntryProvider, LogEntryProvider>();
        services.AddTransient<IMemoryCache, MemoryCache>();
        services.AddTransient<IQrCodeFactory, QrCodeFactory>();
        services.AddTransient<IAccountPlanFactory, AccountPlanFactory>();
        services.AddTransient<INotificationHandler, NotificationHandler>();
        services.AddTransient<ICreatureEventProvider, CreatureEventProvider>();
            
        services.AddScoped<INotificationProcessorFactory<INotificationAcceptanceProcessor>, NotificationAcceptanceProcessorFactory>();

        services.AddScoped<DigimalAwaitingTransferToAcceptanceProcessor>()
            .AddScoped<INotificationAcceptanceProcessor, DigimalAwaitingTransferToAcceptanceProcessor>(s => s.GetService<DigimalAwaitingTransferToAcceptanceProcessor>());
            
            
        services.AddScoped<INotificationProcessorFactory<INotificationDeclineProcessor>, NotificationDeclineProcessorFactory>();
            
        services.AddScoped<DigimalAwaitingTransferToDeclinedProcessor>()
            .AddScoped<INotificationDeclineProcessor, DigimalAwaitingTransferToDeclinedProcessor>(s => s.GetService<DigimalAwaitingTransferToDeclinedProcessor>());
        
        services.Configure<Auth0Settings>(
            configuration.GetSection("Auth0Settings"));
    } 
        
}
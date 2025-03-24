using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyDigimal.Data
{
    public static class Register
    {
        /// <summary>
        /// Adds dependencies to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add services to.</param>
        public static void InitializeDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NpgSqlConnectionConfig>(
                configuration.GetSection("NpgSqlConnectionConfig"));
            
            services.AddTransient<IDbContext, NpgSqlDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        } 
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDigimal.Common.Cryptography;
using MyDigimal.Common.Cryptography.Models;
using MyDigimal.Common.Images;

namespace MyDigimal.Common
{
    public static class Register
    {
        /// <summary>
        /// Adds dependencies to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add services to.</param>
        /// <param name="configuration"></param>
        public static void InitializeCommonDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<EncryptionConfig>(
                configuration.GetSection("EncryptionConfig"));

            services.AddSingleton<IEncryptor, Encryptor>();
            services.AddSingleton<IImageProcessor, ImageProcessor>();
        }
    }
}
using Agibank.Domain.Interfaces;
using Agibank.Domain.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Agibank.Shared
{
    public static class DIResolverExtension
    {
        public static IServiceCollection ConfigureDefaultDI(this IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IAnaliseVendasService, AnaliseVendasService>();
            return services;
        }
    }
}

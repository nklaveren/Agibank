using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using HealthChecks.UI.Client;
using Agibank.Domain.Interfaces;
using Agibank.Domain.Services;
using Agibank.EventBus.Consumer.Tasks;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Agibank.EventBus.Consumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddDistributedMemoryCache();
            services.Configure<ConsumerSettings>(options => Configuration.GetSection(nameof(ConsumerSettings)).Bind(options));
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
            services.AddOptions()
                .AddScoped<IFileService, FileService>()
                .AddHostedService<AnaliseVendasWorker>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}

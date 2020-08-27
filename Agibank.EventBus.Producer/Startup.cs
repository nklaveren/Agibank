using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Agibank.EventBus.Producer.Tasks;
using Agibank.Domain.Services;
using Agibank.Domain.Interfaces;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Agibank.EventBus.Producer
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
            //aqui usaria um ncache ou redis para ambientes distribuidos
            services.AddDistributedMemoryCache();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
            services.Configure<ProducerSettings>(options => Configuration.GetSection(nameof(ProducerSettings)).Bind(options));
            services.AddOptions()
                .AddScoped<IFileService, FileService>()
                .AddHostedService<FileReaderWorker>();
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

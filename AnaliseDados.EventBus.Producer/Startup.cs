using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using AnaliseDados.EventBus.Producer.Tasks;
using AnaliseDados.Domain.Services;
using AnaliseDados.Domain.Interfaces;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace AnaliseDados.EventBus.Producer
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
                .AddScoped<IArquivoService, ArquivoService>()
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

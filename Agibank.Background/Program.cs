using Agibank.Background.Reports.AnalysisSellers;
using Agibank.Domain.Interfaces;
using Agibank.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Agibank.Background
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AnalysisSellersConfiguration>(hostContext.Configuration.GetSection("AnalysisSellersConfiguration"));
                    services.AddSingleton<IFileService, FileService>();
                    services.AddSingleton<IAnalysisSellersService, AnalysisSellersService>();
                    services.AddHostedService<AnalysisSellersWorker>();
                });
            return hostBuilder;
        }
    }
}

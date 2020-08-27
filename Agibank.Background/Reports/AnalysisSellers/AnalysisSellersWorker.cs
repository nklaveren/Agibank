using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agibank.Domain.DTOs;
using Agibank.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agibank.Background.Reports.AnalysisSellers
{
    public sealed class AnalysisSellersWorker : BackgroundService
    {
        private const string INVALIDCONFIGURATION = "Arquivo de configuração inválido";
        private const string ALREADYPROCESSED = "Arquivo {0} já processado";
        private const string UNESPECTEDERROR = "Erro não esperado";
        private readonly ILogger<AnalysisSellersWorker> _logger;
        private readonly AnalysisSellersConfiguration config;
        private readonly IFileService fileService;
        private readonly IAnalysisSellersService analysisSellersService;
        readonly ConcurrentDictionary<string, AnalysisSellersRequest> _cache = new ConcurrentDictionary<string, AnalysisSellersRequest>();
        public AnalysisSellersWorker(ILogger<AnalysisSellersWorker> logger,
            IOptions<AnalysisSellersConfiguration> configuration,
            IFileService fileService,
            IAnalysisSellersService analysisSellersService)
        {
            _logger = logger;
            this.config = configuration.Value;
            this.fileService = fileService;
            this.analysisSellersService = analysisSellersService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (config is null || !config.IsValid())
                {
                    throw new Exception(INVALIDCONFIGURATION);
                }
                try
                {
                    var files = fileService.GetAllFiles(config.PathIn, config.FileExtension);
                    files.Select(async file =>
                    {
                        var fileName = await fileService.GetFileName(file);
                        string[] content = await fileService.GetFileContent(file);
                        var request = new AnalysisSellersRequest(content);

                        if (config.ReProcessFile || _cache.TryAdd(fileName, request))
                        {
                            _logger.LogInformation($"Begin Proc File {fileName}");
                            try
                            {
                                var response = await analysisSellersService.Execute(request);
                                if (response.Ok)
                                {
                                    var fileContent = FormartReport(response);
                                    var outputFilename = string.Format(config.OutputFilename, fileName);
                                    await fileService.WriteFile(fileContent, config.PathOut, outputFilename);
                                    _logger.LogInformation($"{fileName} ok");
                                }
                                else
                                {
                                    _logger.LogError($"{fileName} bad file");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"{fileName} * bad file");
                            }

                            _logger.LogInformation($"End Proc File {fileName}");
                        }
                        else
                        {
                            if (config.WarningAlreadyProcessedFiles)
                                _logger.LogInformation(ALREADYPROCESSED, fileName);
                        }
                    });

                    await Task.Delay(new TimeSpan(0, 0, 1));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, UNESPECTEDERROR);
                }
            }
        }

        private const string SEPARETOR = "\n";
        private string FormartReport(AnalysisSellersResponse response)
        {
            var includeResult = new List<string>
            {
                string.Empty.PadLeft(33,'-'),
                $"RESULTADO DA ANÁLISE DE DADOS:  ",
                $"CLIENTES           : {response.Clients} ",
                $"VENDEDORES         : {response.Sellers }",
                $"ID VENDA MAIS CARA : {response.BestSell}",
                $"PIOR VENDEDOR      : {response.SalesMan}",
                string.Empty.PadLeft(33,'-'),
            };
            return string.Join(SEPARETOR, includeResult);
        }
    }
}

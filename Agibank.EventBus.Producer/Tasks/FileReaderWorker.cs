using Agibank.Domain.Interfaces;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agibank.EventBus.Producer.Tasks
{
    public sealed class FileReaderWorker : BackgroundService
    {
        #region DI / CTOR

        private readonly IFileService fileService;
        private readonly ILogger<FileReaderWorker> logger;
        readonly List<string> cache = new List<string>();
        private readonly ProducerSettings config;

        public FileReaderWorker(
            IFileService fileService,
            ILogger<FileReaderWorker> logger,
            IOptions<ProducerSettings> config
            )
        {
            this.fileService = fileService;
            this.logger = logger;
            this.config = config.Value;
        }

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = config.Hostname, Port = config.Port };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: config.QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                while (!stoppingToken.IsCancellationRequested)
                {

                    var arquivos = fileService.GetAllFiles(config.PathIn, config.Extension);
                    var arquivosProcessar = arquivos.Except(cache).ToList();

                    if (!arquivosProcessar.Any())
                    {
                        await Task.Delay(100);
                    }
                    arquivosProcessar.ForEach(arquivo =>
                    {
                        //não estou enviando linha a linha do arquivo para não gerar complexidade em um teste
                        logger.LogInformation($"O arquivo {arquivo} foi enviado para o rabbitmq");
                        var arquivoSemExtensao = arquivo.Replace(config.Extension, string.Empty);
                        var body = Encoding.UTF8.GetBytes(arquivoSemExtensao);
                        channel.BasicPublish(exchange: "",
                            routingKey: config.QueueName,
                            basicProperties: null,
                            body: body);
                        cache.Add(arquivo);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro no serviço: { nameof(FileReaderWorker)}");
                await Task.Delay(100);
                await ExecuteAsync(stoppingToken);
            }
        }
    }
}
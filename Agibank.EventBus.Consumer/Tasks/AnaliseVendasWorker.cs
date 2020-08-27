using Agibank.Domain.Interfaces;
using Agibank.Domain.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agibank.EventBus.Consumer.Tasks
{
    public sealed class AnaliseVendasWorker : BackgroundService
    {
        #region DI / CTOR
        private readonly ILogger<AnaliseVendasWorker> logger;
        private readonly IFileService fileService;
        private readonly ConsumerSettings config;

        public AnaliseVendasWorker(
            ILogger<AnaliseVendasWorker> logger,
            IOptions<ConsumerSettings> options,
            IFileService fileService
            )
        {
            this.logger = logger;
            this.fileService = fileService;
            this.config = options.Value;
        }

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () =>
             {
                 try
                 {
                     var factory = new ConnectionFactory() { HostName = config.Hostname, Port = config.Port };
                     var connection = factory.CreateConnection();
                     var channel = connection.CreateModel();
                     ushort.TryParse(config.Channels.ToString(), out ushort prefetch);
                     channel.BasicQos(0, prefetch, false);
                     channel.QueueDeclare(queue: config.QueueName,
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

                     for (int i = 1; i <= config.Channels; i++)
                     {
                         CreateChannel(i, stoppingToken, channel);
                     }
                 }
                 catch (Exception ex)
                 {
                     logger.LogError(ex, $"Erro no serviço {nameof(AnaliseVendasWorker)}");
                     if (!stoppingToken.IsCancellationRequested)
                     {
                         await Task.Delay(100);
                         await ExecuteAsync(stoppingToken);
                     }
                 }
             }).ConfigureAwait(true);
        }

        private void CreateChannel(int index, CancellationToken stoppingToken, IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    logger.LogInformation($"Worker {index} recebeu o arquivo {message}");
                    ProcessaArquivo(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false,true);
                    logger.LogError(ex, nameof(AnaliseVendasWorker));
                }
            };

            channel.BasicConsume(queue: config.QueueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        private void ProcessaArquivo(string arquivoNome)
        {
            try
            {
                var file = Path.Combine(config.PathIn, arquivoNome);
                Stream stream = fileService.GetFileContent(file + config.Extension);
                var streamReader = new StreamReader(stream);
                string linha = string.Empty;
                var construtor = new AnaliseVendasConstrutor();
                while ((linha = streamReader.ReadLine()) != null)
                {
                    construtor.Add(linha);
                }
                streamReader.Close();
                var result = new AnaliseVendasRelatorio(construtor);
                var arquivoSaidaNome = string.Format(config.OutputFilename, arquivoNome);
                fileService.WriteFile(result.ToString(), config.PathOut, arquivoSaidaNome);
                logger.LogInformation($"o arquivo {arquivoNome} foi processado");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(AnaliseVendasWorker));
            }
        }
    }
}


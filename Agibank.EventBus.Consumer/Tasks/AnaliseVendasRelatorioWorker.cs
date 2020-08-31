using Agibank.Domain.Interfaces;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agibank.EventBus.Consumer.Tasks
{
    public sealed class AnaliseVendasRelatorioWorker : BackgroundService
    {
        #region DI / CTOR

        private readonly ILogger<AnaliseVendasRelatorioWorker> logger;
        private readonly IProcessaArquivoService processaArquivoService;
        private readonly ConsumerSettings config;

        public AnaliseVendasRelatorioWorker(
            ILogger<AnaliseVendasRelatorioWorker> logger,
            IProcessaArquivoService gerarRelatorioService,
            IOptions<ConsumerSettings> config)
        {
            this.logger = logger;
            this.processaArquivoService = gerarRelatorioService;
            this.config = config.Value;
        }

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = config.Hostname, Port = config.Port };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                channel.BasicQos(0, 1, false);
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
                logger.LogError(ex, $"Erro no serviço {nameof(AnaliseVendasRelatorioWorker)}");
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(100);
                    await ExecuteAsync(stoppingToken);
                }
            }
        }

        private void CreateChannel(int index, CancellationToken stoppingToken, IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    logger.LogInformation($"Worker {index} recebeu o arquivo {message}");
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    await processaArquivoService.ProcessaArquivo(message, config.PathIn, config.PathOut);
                    stopwatch.Stop();

                    channel.BasicAck(ea.DeliveryTag, false);
                    logger.LogInformation($"Worker {index} processou o arquivo {message} em {stopwatch.Elapsed.TotalSeconds } segundos ");
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    logger.LogError(ex, nameof(AnaliseVendasRelatorioWorker));
                }
            };

            channel.BasicConsume(queue: config.QueueName,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}

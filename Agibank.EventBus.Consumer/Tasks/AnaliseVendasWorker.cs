using Agibank.Domain.Builders;
using Agibank.Domain.Interfaces;
using Agibank.Domain.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                logger.LogError(ex, $"Erro no serviço {nameof(AnaliseVendasWorker)}");
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
                    await ProcessaArquivo(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    logger.LogError(ex, nameof(AnaliseVendasWorker));
                }
            };

            channel.BasicConsume(queue: config.QueueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        private async Task ProcessaArquivo(string arquivoNome)
        {
            try
            {
                var construtor = new AnaliseVendasBuilder();
                var file = Path.Combine(config.PathIn, arquivoNome);

                await LerLinhaPorLinha(construtor, file);

                var analise = new AnaliseVendasRelatorio()
                    .ComClientes(construtor.Clientes)
                    .ComVendedores(construtor.Vendedores)
                    .ComVendas(construtor.Vendas)
                    .Construir();

                var arquivoSaidaNome = string.Format(config.OutputFilename, arquivoNome);
                await fileService.WriteFile(analise.ToString(), config.PathOut, arquivoSaidaNome);
                construtor.Dispose();
                analise.Dispose();
                logger.LogInformation($"o arquivo {arquivoNome} foi processado");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(AnaliseVendasWorker));
            }
        }

        private async Task LerLinhaPorLinha(AnaliseVendasBuilder construtor, string file)
        {
            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
            Stream stream = fileService.GetFileContent(file + config.Extension);
            var streamReader = new StreamReader(stream);

            string linha;

            while ((linha = await streamReader.ReadLineAsync()) != null)
            {
                queue.Enqueue(linha);
            }
            streamReader.Close();
            Parallel.Invoke(() =>
            {
                while (queue.TryDequeue(out var linhaQueue))
                {
                    construtor.Add(linhaQueue);
                }
            });
        }
    }
}


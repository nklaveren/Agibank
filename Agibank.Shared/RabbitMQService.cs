using Agibank.Domain.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Agibank.Shared
{
    public class RabbitMQService : IEventBusService
    {
        private readonly RabbitMQConfig config;
        private readonly ILogger<RabbitMQService> logger;

        public Func<string, Task> OnReceive { get; set; }

        public RabbitMQService(ILogger<RabbitMQService> logger, IOptions<RabbitMQConfig> config)
        {
            this.config = config.Value;
            this.logger = logger;
        }
        public async Task SendAsync(string nomeArquivo)
        {
            try
            {

                var factory = new ConnectionFactory() { HostName = config.Hostname };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: config.QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var body = Encoding.UTF8.GetBytes(nomeArquivo);
                /**
                 * Não enviei o conteúdo do arquivo, pois se o arquivo for muito grande
                 * iria gerar uma sobrecarga neste serviço
                 * uma forma mais trabalhosa seria necessária... (queue por linha + multithread)
                 * e no consumidor (cache distribuido + controle linha a linha do arquivo)
                **/
                await Task.Run(() =>
                {
                    channel.BasicPublish(exchange: "",
                                         routingKey: config.QueueName,
                                         basicProperties: null,
                                         body: body);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro no serviço: { nameof(RabbitMQService)}");
            }
        }
    }
}

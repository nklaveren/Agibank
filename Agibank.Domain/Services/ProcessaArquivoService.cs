using Agibank.Domain.Interfaces;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Agibank.Domain.Services
{
    public class ProcessaArquivoService : IProcessaArquivoService
    {
        #region DI / CTOR

        private readonly IRelatorioService relatorioService;
        private readonly IArquivoService arquivoService;

        public ProcessaArquivoService(IRelatorioService relatorioService, IArquivoService arquivoService)
        {
            this.relatorioService = relatorioService;
            this.arquivoService = arquivoService;
        }

        #endregion

        public async Task ProcessaArquivo(string arquivoNome, string pathIn, string pathOut)
        {
            try
            {
                ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
                string arquivoNomeCompleto = $"{arquivoNome}.{relatorioService.RelatorioExtensao}";
                Stream stream = arquivoService.ObtemConteudo(pathIn, arquivoNomeCompleto);
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
                        relatorioService.Adicionar(linhaQueue);
                    }
                });

                var relatorio = relatorioService.Processar();
                var arquivoSaidaNome = $"{arquivoNome}.done.{relatorioService.RelatorioExtensao}";
                await arquivoService.Escrever(relatorio.ToString(), pathOut, arquivoSaidaNome);
                relatorioService.Dispose();
            }
            catch (Exception ex)
            {
                ex.Data.Add("GerarRelatorioService", ex.ToString());
                throw ex;
            }
        }
    }
}

using Agibank.Domain.Interfaces;
using Agibank.Domain.Services;
using Agibank.Domain.Tests.Services;

using System;
using System.IO;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Analytics.UserId;

using Xunit;

namespace Agibank.Domain.Tests.Steps
{
    [Binding]
    public class ProcessarArquivoSteps
    {
        private IProcessaArquivoService servico;
        private IArquivoService arquivoService;
        private string extensao;
        private string arquivoNome;
        private string pastaSaida;

        [BeforeScenario]
        public void Init()
        {
            servico = MockServices.ProcessaArquivoService;
            arquivoService = MockServices.ArquivoService;
            extensao = MockServices.AnaliseVendasService.RelatorioExtensao;
        }

        [AfterScenario]
        public void Close()
        {
            servico = null;
        }

        [Given(@"um arquivo '(.*)' no diretorio '(.*)' e com diretorio de saida '(.*)'")]
        public void DadoUmArquivoNoDiretorioEComDiretorioDeSaida(string arquivoNome, string pastaEntrada, string pastaSaida)
        {
            this.arquivoNome = arquivoNome;
            this.pastaSaida = pastaSaida;
            servico.ProcessaArquivo(arquivoNome, pastaEntrada, pastaSaida);
        }

        [Then(@"Gero um arquivo de saida com conteudo '(.*)'")]
        public void EntaoGeroUmArquivoDeSaidaComConteudo(string esperado)
        {
            var nomeArquivo = $"{arquivoNome}.done.{extensao}";
            var stream = arquivoService.ObtemConteudo(pastaSaida, nomeArquivo);
            var conteudo = new StreamReader(stream).ReadToEnd();
            Assert.Equal(esperado, conteudo);
        }
    }
}

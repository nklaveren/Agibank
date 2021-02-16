using AnaliseDados.Domain.Interfaces;
using AnaliseDados.Domain.Tests.Services;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Host;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TechTalk.SpecFlow;

using Xunit;

namespace AnaliseDados.Domain.Tests.Steps
{
    [Binding]
    public class LerUmArquivoSteps
    {
        private string texto;
        private IArquivoService arquivoService;
        private string fileName;
        private string diretorio;
        private IEnumerable<string> files;
        private string fileContent;

        [BeforeScenario]
        public void Init()
        {
            this.texto = string.Empty;

            this.arquivoService = MockServices.ArquivoService;
        }

        [AfterScenario]
        public void Close()
        {
            this.texto = null;
        }

        [Given(@"que tenho o diretório '(.*)'")]
        public void DadoQueTenhoODiretorio(string diretorio)
        {
            this.diretorio = diretorio;
        }

        [Given(@"que tenho um texto '(.*)'")]
        public void DadoQueTenhoUmTexto(string texto)
        {
            this.texto = texto;
        }

        [Then(@"o sistema deve ser capaz de gravar um arquivo nome: '(.*)' e extensão '(.*)'")]
        public async Task EntaoOSistemaDeveSerCapazDeGravarUmArquivoNomeEExtensao(string arquivo, string extensao)
        {
            fileName = $"{arquivo}.{extensao}";
            await this.arquivoService.Escrever(this.texto, this.diretorio, fileName);
            var saved = await this.arquivoService.Existe(this.diretorio, fileName);
            Assert.True(saved);
        }

        [Then(@"tenho (.*) arquivos salvos")]
        public void EntaoTenhoArquivosSalvos(int quantidade)
        {
            var arquivos = this.arquivoService.ObtemTodos(this.diretorio, "teste");

            Assert.Equal(quantidade, arquivos.Count());
        }


        [Given(@"que tenho o campo nome '(.*)'")]
        public void DadoQueTenhoOCampoNome(string p0)
        {
            this.fileName = p0;
        }


        [Given(@"leio o conteúdo do arquivo")]
        public async Task DadoLeioOConteudoDoArquivoAsync()
        {
            var stream = this.arquivoService.ObtemConteudo(".", this.fileName);
            var sr = new StreamReader(stream);
            this.fileContent = await sr.ReadLineAsync();
        }

        [Then(@"o conteúdo lido deve ser '(.*)'")]
        public void EntaoOConteudoLidoDeveSer(string conteudo)
        {
            Assert.Equal(conteudo, this.fileContent);
        }

        [Given(@"que tenho (.*) arquivos")]
        public void DadoQueTenhoArquivos(int p0)
        {
            var arquivos = MockServices.ArquivoService.ObtemTodos(this.diretorio, "dat");
            Assert.Equal(2, arquivos.Count());
        }

        [Then(@"removo os dois")]
        public void EntaoRemovoOsDois()
        {
            var arquivos = MockServices.ArquivoService.ObtemTodos(this.diretorio, "dat");
            foreach (var arquivo in arquivos)
            {
                var arquivoNome = Path.Combine(diretorio, arquivo);
                MockServices.ArquivoService.Remover(arquivoNome);
            }
        }

        [Then(@"não encontro mais nenhum arquivo")]
        public void EntaoNaoEncontroMaisNenhumArquivo()
        {
            var arquivos = MockServices.ArquivoService.ObtemTodos(this.diretorio, "dat");
            Assert.True(!arquivos.Any());
        }
    }
}

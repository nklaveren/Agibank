using AnaliseDados.Domain.Interfaces;

using Microsoft.Extensions.FileProviders;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnaliseDados.Domain.Services
{
    public class ArquivoService : IArquivoService
    {
        private readonly IFileProvider provider;

        public ArquivoService(IFileProvider provider)
        {
            this.provider = provider;
        }
        public IEnumerable<string> ObtemTodos(string diretorio, string extensao)
        {
            var files = provider.GetDirectoryContents(diretorio).Where(x => x.Name.EndsWith(extensao));

            foreach (var item in files)
            {
                yield return item.Name;
            }
        }

        public Stream ObtemConteudo(string diretorio, string arquivoNomeCompleto)
        {
            var file = Path.Combine(diretorio, arquivoNomeCompleto);
            if (File.Exists(file))
            {
                return provider.GetFileInfo(file).CreateReadStream();
            }
            else return default;
        }

        public async Task Escrever(string conteudo, string diretorio, string nome)
        {
            var arquivo = Path.Combine(diretorio, nome);
            Remover(arquivo);
            await File.WriteAllTextAsync(arquivo, conteudo);
        }

        public void Remover(string nome)
        {
            if (File.Exists(nome))
            {
                File.Delete(nome);
            }
        }

        public Task<bool> Existe(string diretorio, string nome)
        {
            return Task.FromResult(File.Exists(Path.Combine(diretorio, nome)));
        }
    }
}

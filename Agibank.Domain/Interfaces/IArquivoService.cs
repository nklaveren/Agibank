using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Agibank.Domain.Interfaces
{
    public interface IArquivoService
    {
        IEnumerable<string> ObtemTodos(string diretorio, string extensao);
        Stream ObtemConteudo(string diretorio, string arquivoNomeCompleto);
        Task Escrever(string conteudo, string diretorio, string nome);
        void Remover(string nome);
        Task<bool> Existe(string diretorio, string nome);
    }
}

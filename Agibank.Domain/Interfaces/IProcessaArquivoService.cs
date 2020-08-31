
using System.Threading.Tasks;

namespace Agibank.Domain.Interfaces
{
    public interface IProcessaArquivoService 
    {
        Task ProcessaArquivo(string arquivoNome, string pastaEntrada, string pastaSaida);
    }
}
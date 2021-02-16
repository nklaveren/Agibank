
using System.Threading.Tasks;

namespace AnaliseDados.Domain.Interfaces
{
    public interface IProcessaArquivoService 
    {
        Task ProcessaArquivo(string arquivoNome, string pastaEntrada, string pastaSaida);
    }
}
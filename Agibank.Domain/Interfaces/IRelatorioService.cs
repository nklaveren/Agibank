using System;

namespace Agibank.Domain.Interfaces
{
    public interface IRelatorioService : IDisposable
    {
        void Adicionar(string item);

        IRelatorio Processar();

        string RelatorioExtensao { get; }
    }
}

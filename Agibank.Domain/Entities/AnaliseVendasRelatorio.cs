using Agibank.Domain.Interfaces;

namespace Agibank.Domain.Entities
{
    public class AnaliseVendasRelatorio : IRelatorio
    {
        public string PiorVendedor { get; set; }
        public long MelhorVenda { get; set; }
        public int Vendedores { get; set; }
        public int Clientes { get; set; }

        public override string ToString()
        {
            var list = new[]
            {
                Clientes.ToString(),
                Vendedores.ToString(),
                MelhorVenda.ToString(),
                PiorVendedor.ToString()
            };
            const string SEPARADOR = "ç";
            var result = string.Join(SEPARADOR, list).Trim();
            return result;
        }
    }
}

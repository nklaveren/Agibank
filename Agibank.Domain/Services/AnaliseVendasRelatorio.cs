using System.Linq;

namespace Agibank.Domain.Services
{
    public class AnaliseVendasRelatorio
    {
        public AnaliseVendasRelatorio(AnaliseVendasConstrutor builder)
        {
            Vendedores = builder.Vendedores.Count;
            Clientes = builder.Clientes.Count;
            var maiorVenda = builder.Vendas.OrderByDescending(x => x.Total).FirstOrDefault();
            if (maiorVenda != null)
            {
                MelhorVenda = maiorVenda.Id;
            }

            var piorVendedor = builder.Vendas.GroupBy(x => x.VendedorNome)
                .Select(x => new { Vendedor = x.Key, Total = x.Sum(_ => _.Total) })
                .OrderBy(x => x.Total)
                .FirstOrDefault();
            if (piorVendedor != null)
            {
                PiorVendedor = piorVendedor.Vendedor;
            }
        }

        public string PiorVendedor { get; }
        public long MelhorVenda { get; }
        public int Vendedores { get; }
        public int Clientes { get; }


        public override string ToString()
        {
            return $"{Clientes}ç{Vendedores}ç{MelhorVenda}ç{PiorVendedor}";
        }
    }
}

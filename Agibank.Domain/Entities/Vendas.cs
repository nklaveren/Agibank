using System.Collections.Generic;
using System.Linq;

namespace Agibank.Domain.Entities
{
    public class Vendas
    {
        public Vendas(long id, string vendedorNome, List<VendasItem> items)
        {
            this.Id = id;
            this.VendedorNome = vendedorNome;
            this.Items = items;
        }

        public long Id { get; set; }
        public List<VendasItem> Items { get; set; }
        public string VendedorNome { get; set; }
        public decimal Total => Items.Sum(x => x.Total);
    }
}

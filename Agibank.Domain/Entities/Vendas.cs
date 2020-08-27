using System.Collections.Generic;
using System.Linq;

namespace Agibank.Domain.Entities
{
    public class Vendas
    {
        public Vendas(string[] itemSplit, List<VendasItem> items) : this(
            id: long.Parse(itemSplit[1]), vendedorNome: itemSplit[3], items: items ?? new List<VendasItem>())
        { }

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

using Agibank.Domain.Entities;

using System.Collections.Generic;
using System.Globalization;

namespace Agibank.Domain.Builders
{
    public class VendasBuilder
    {
        string id, vendedorNome, itens;

        public VendasBuilder ComId(string id)
        {
            this.id = id;
            return this;
        }

        public VendasBuilder ComVendedorNome(string vendedorNome)
        {
            this.vendedorNome = vendedorNome;
            return this;
        }

        public VendasBuilder ComVendaItens(string vendaItens)
        {
            this.itens = vendaItens;
            return this;
        }

        public Vendas Construir()
        {
            var idLong = long.Parse(id, CultureInfo.InvariantCulture);
            var itens = ConstruirVendaItens();
            return new Vendas(idLong, vendedorNome, itens);
        }

        private List<VendasItem> ConstruirVendaItens()
        {
            var resultado = new List<VendasItem>();
            if (!string.IsNullOrEmpty(itens))
            {
                var vendasItemTratadoSplit = itens.Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
                foreach (var item in vendasItemTratadoSplit)
                {
                    var itemSplit = item.Split("-");
                    var vendasItem = new VendasItem(
                        id: int.Parse(itemSplit[0], CultureInfo.InvariantCulture),
                        quantidade: long.Parse(itemSplit[1], CultureInfo.InvariantCulture),
                        preco: decimal.Parse(itemSplit[2], CultureInfo.InvariantCulture));
                    resultado.Add(vendasItem);
                }
            }

            return resultado;
        }
    }
}

using Agibank.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Agibank.Domain.Services
{
    public sealed class AnaliseVendasConstrutor
    {
        public const char SEPARADOR = 'ç';
        private const string VENDEDOR = "001";
        private const string CLIENTE = "002";
        private const string VENDAS = "003";

        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<Vendas> Vendas { get; set; } = new List<Vendas>();
        public List<Vendedor> Vendedores { get; set; } = new List<Vendedor>();


        public void Add(string item)
        {
            var itemSplit = item.Split(AnaliseVendasConstrutor.SEPARADOR);
            var tipo = itemSplit[0];
            switch (tipo)
            {
                case VENDEDOR:
                    //001ç1234567891234çPedroç50000
                    Vendedores.Add(new Vendedor(
                        cpf: itemSplit[1],
                        nome: itemSplit[2],
                        Salario: decimal.Parse(itemSplit[3], CultureInfo.InvariantCulture))
                        );
                    break;
                case CLIENTE:
                    //002ç2345675433444345çEduardo PereiraçRural
                    Clientes.Add(new Cliente(
                        cnpj: itemSplit[1],
                        name: itemSplit[2],
                        areaNegocio: itemSplit[3]));
                    break;
                case VENDAS:
                    //003ç10ç[1-10-100,2-30-2.50,3-40-3.10]çPedro
                    Vendas.Add(new Vendas(
                        id: long.Parse(itemSplit[1], CultureInfo.InvariantCulture),
                        vendedorNome: itemSplit[3],
                        items: ExtrairVendasItem(itemSplit[2]))
                        );
                    break;
            };
        }

        private List<VendasItem> ExtrairVendasItem(string vendaItemString)
        {
            var resultado = new List<VendasItem>();
            var vendasItemTratadoSplit = vendaItemString.Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
            foreach (var item in vendasItemTratadoSplit)
            {
                var itemSplit = item.Split("-");
                var vendasItem = new VendasItem(
                    id: int.Parse(itemSplit[0], CultureInfo.InvariantCulture),
                    quantidade: long.Parse(itemSplit[1], CultureInfo.InvariantCulture),
                    preco: decimal.Parse(itemSplit[2], CultureInfo.InvariantCulture));
                resultado.Add(vendasItem);
            }

            return resultado;
        }
    }
}
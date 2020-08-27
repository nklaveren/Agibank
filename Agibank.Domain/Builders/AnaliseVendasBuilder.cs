using Agibank.Domain.Entities;

using System;
using System.Collections.Generic;

namespace Agibank.Domain.Builders
{
    public sealed class AnaliseVendasBuilder : IDisposable
    {
        public const char SEPARADOR = 'ç';
        private const string VENDEDOR = "001";
        private const string CLIENTE = "002";
        private const string VENDAS = "003";

        public AnaliseVendasBuilder()
        {
        }

        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<Vendas> Vendas { get; set; } = new List<Vendas>();
        public List<Vendedor> Vendedores { get; set; } = new List<Vendedor>();


        public void Add(string item)
        {
            var itemSplit = item.Split(AnaliseVendasBuilder.SEPARADOR);
            var tipo = itemSplit[0];
            switch (tipo)
            {
                case VENDEDOR:
                    Vendedores.Add(
                            new VendedorBuilder()
                                .ComCpf(itemSplit[1])
                                .ComNome(itemSplit[2])
                                .ComSalario(itemSplit[3])
                                .Construir());
                    break;

                case CLIENTE:
                    Clientes.Add(
                        new ClienteBuilder()
                            .ComCnpj(itemSplit[1])
                            .ComNome(itemSplit[2])
                            .ComAreaNegocio(itemSplit[3])
                            .Construir());
                    break;

                case VENDAS:
                    Vendas.Add(new VendasBuilder()
                            .ComId(itemSplit[1])
                            .ComVendaItens(itemSplit[2])
                            .ComVendedorNome(itemSplit[3])
                            .Construir());
                    break;
            };
        }

        public void Dispose()
        {
            this.Clientes = null;
            this.Vendas = null;
            this.Vendedores = null;
            GC.Collect();
        }
    }
}
using Agibank.Domain.Builders;
using Agibank.Domain.Entities;
using Agibank.Domain.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Agibank.Domain.Services
{
    public class AnaliseVendasRelatorioService : IRelatorioService, IDisposable
    {
        public const char SEPARADOR = 'ç';
        private const string VENDEDOR = "001";
        private const string CLIENTE = "002";
        private const string VENDAS = "003";

        public string RelatorioExtensao => "dat";

        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<Vendas> Vendas { get; set; } = new List<Vendas>();
        public List<Vendedor> Vendedores { get; set; } = new List<Vendedor>();

        public void Adicionar(string item)
        {
            var itemSplit = item.Split(AnaliseVendasRelatorioService.SEPARADOR);
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

        public IRelatorio Processar()
        {
            var relatorio = new AnaliseVendasRelatorio();
            if (Clientes != null)
            {
                relatorio.Clientes = Clientes.Count;
            }

            if (Vendedores != null)
            {
                relatorio.Vendedores = Vendedores.Count;
            }
            if (Vendas != null)
            {
                var maiorVenda = Vendas.OrderByDescending(x => x.Total).FirstOrDefault();
                if (maiorVenda != null)
                {
                    relatorio.MelhorVenda = maiorVenda.Id;
                }

                var piorVendedor = Vendas.GroupBy(x => x.VendedorNome)
                    .Select(x => new { Vendedor = x.Key, Total = x.Sum(_ => _.Total) })
                    .OrderBy(x => x.Total)
                    .FirstOrDefault();

                if (piorVendedor != null)
                {
                    relatorio.PiorVendedor = piorVendedor.Vendedor;
                }
            }
            return relatorio;
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
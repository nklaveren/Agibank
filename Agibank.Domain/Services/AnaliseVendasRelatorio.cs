using System;
using System.Collections.Generic;
using System.Linq;

namespace Agibank.Domain.Services
{
    public class AnaliseVendasRelatorio
    {
        const string SEPARADOR = "ç";
        private List<Entities.Cliente> _clientes;
        private List<Entities.Vendedor> _vendedores;
        private List<Entities.Vendas> _vendas;

        public string PiorVendedor { get; set; }
        public long MelhorVenda { get; set; }
        public int Vendedores { get; set; }
        public int Clientes { get; set; }


        public AnaliseVendasRelatorio ComClientes(List<Entities.Cliente> clientes)
        {
            _clientes = clientes;
            return this;
        }

        public AnaliseVendasRelatorio ComVendedores(List<Entities.Vendedor> vendedores)
        {
            _vendedores = vendedores;
            return this;
        }

        public AnaliseVendasRelatorio ComVendas(List<Entities.Vendas> vendas)
        {
            _vendas = vendas;
            return this;
        }

        public override string ToString()
        {
            return $"{Clientes}{SEPARADOR}{Vendedores}{SEPARADOR}{MelhorVenda}{SEPARADOR}{PiorVendedor}";
        }

        public AnaliseVendasRelatorio Construir()
        {
            if (_clientes != null)
            {
                this.Clientes = _clientes.Count;
            }

            if (_vendedores != null)
            {
                this.Vendedores = _vendedores.Count;
            }
            if (_vendas != null)
            {
                var maiorVenda = _vendas.OrderByDescending(x => x.Total).FirstOrDefault();
                if (maiorVenda != null)
                {
                    MelhorVenda = maiorVenda.Id;
                }

                var piorVendedor = _vendas.GroupBy(x => x.VendedorNome)
                    .Select(x => new { Vendedor = x.Key, Total = x.Sum(_ => _.Total) })
                    .OrderBy(x => x.Total)
                    .FirstOrDefault();

                if (piorVendedor != null)
                {
                    PiorVendedor = piorVendedor.Vendedor;
                }
            }
            return this;
        }

        public void Dispose()
        {
            _vendas = null;
            _clientes = null;
            _vendedores = null;
            GC.Collect();
        }
    }
}

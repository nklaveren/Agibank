using Agibank.Domain.Builders;
using Agibank.Domain.Services;

using System.Globalization;
using System.Linq;

using TechTalk.SpecFlow;

using Xunit;

namespace Agibank.Domain.Tests.Steps
{
    [Binding]
    public class AnaliseVendasSteps
    {
        string linha;
        AnaliseVendasBuilder construtor;

        [BeforeScenario]
        public void Init()
        {
            linha = "";
            construtor = new AnaliseVendasBuilder();
        }

        [AfterScenario]
        public void Close()
        {
            linha = null;
            construtor = null;
        }
        [Given(@"que tenho dados do vendedor")]
        public void DadoQueTenhoDadosDoVendedor(Table table)
        {
            linha = GetLinha(table);
        }

        private string GetLinha(Table table, int row = 0)
        {
            var values = table.Rows[row].Values;
            return values.FirstOrDefault();
        }

        [Then(@"O resultado esperado é uma instancia de um vendedor:")]
        public void EntaoOResultadoEsperadoEUmaInstanciaDeUmVendedor(Table table)
        {
            construtor.Add(linha);

            var vendedor = construtor.Vendedores.FirstOrDefault();
            var assertVendedor = table.Rows[0];

            Assert.Equal(vendedor.Cpf, assertVendedor["CPF"]);
            Assert.Equal(vendedor.Nome, assertVendedor["Nome"]);

            var salario = decimal.Parse(assertVendedor["Salario"], CultureInfo.InvariantCulture);

            Assert.Equal(vendedor.Salario, salario);
        }

        [Given(@"que tenho dados do Cliente")]
        public void DadoQueTenhoDadosDoCliente(Table table)
        {
            linha = GetLinha(table);
        }

        [Then(@"O resultado esperado é uma instancia de um Cliente:")]
        public void EntaoOResultadoEsperadoEUmaInstanciaDeUmCliente(Table table)
        {
            construtor.Add(linha);

            var cliente = construtor.Clientes.FirstOrDefault();
            var assertCliente = table.Rows[0];

            Assert.Equal(cliente.Cnpj, assertCliente["CNPJ"]);
            Assert.Equal(cliente.Nome, assertCliente["Nome"]);
            Assert.Equal(cliente.AreaNegocio, assertCliente["Area Negocio"]);
        }

        [Given(@"que tenho dados da venda")]
        public void DadoQueTenhoDadosDaVenda(Table table)
        {
            linha = GetLinha(table);
        }

        [Then(@"O resultado esperado é uma instancia de uma Venda:")]
        public void EntaoOResultadoEsperadoEUmaInstanciaDeUmaVenda(Table table)
        {
            construtor.Add(linha);

            var vendas = construtor.Vendas.FirstOrDefault();

            var assertVendas = table.Rows[0];

            Assert.Equal(vendas.Id, int.Parse(assertVendas["Venda Id"], CultureInfo.InvariantCulture));
            Assert.Equal(vendas.VendedorNome, assertVendas["Vendedor Nome"]);
        }

        [Then(@"os itens da da venda são:")]
        public void EntaoOsItensDaDaVendaSao(Table table)
        {
            var itensVenda = construtor.Vendas.FirstOrDefault().Items;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var assertItensVendas = table.Rows[i];
                var item = itensVenda[i];

                Assert.Equal(item.Id, int.Parse(assertItensVendas["Item ID"], CultureInfo.InvariantCulture));
                Assert.Equal(item.Quantidade, int.Parse(assertItensVendas["Item Quantidade"], CultureInfo.InvariantCulture));
                Assert.Equal(item.Preco, decimal.Parse(assertItensVendas["Item Preço"], CultureInfo.InvariantCulture));
            }
        }

        [Given(@"que tenho as informações:")]
        public void DadoQueTenhoAsInformacoes(Table table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                construtor.Add(GetLinha(table, i));
            }
        }

        [Then(@"O resultado esperado da analise é:")]
        public void EntaoOResultadoEsperadoDaAnaliseE(Table table)
        {
            var analise = new AnaliseVendasRelatorio()
                    .ComClientes(construtor.Clientes)
                    .ComVendedores(construtor.Vendedores)
                    .ComVendas(construtor.Vendas)
                    .Construir();

            var row = table.Rows[0];

            Assert.Equal(analise.Clientes, int.Parse(row["Quantidade Clientes"]));
            Assert.Equal(analise.Vendedores, int.Parse(row["Quantidade Vendededor"]));
            Assert.Equal(analise.MelhorVenda, int.Parse(row["Venda mais cara"]));
            Assert.Equal(analise.PiorVendedor, row["Pior Vendedor"]);
        }
    }
}

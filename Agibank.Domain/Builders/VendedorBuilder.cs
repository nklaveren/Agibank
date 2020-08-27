using Agibank.Domain.Entities;

using System.Globalization;

namespace Agibank.Domain.Builders
{
    public class VendedorBuilder
    {
        string cpf, nome, salario;
        public VendedorBuilder ComCpf(string cpf)
        {
            this.cpf = cpf;
            return this;
        }

        public VendedorBuilder ComNome(string nome)
        {
            this.nome = nome;
            return this;
        }

        public VendedorBuilder ComSalario(string salario)
        {
            this.salario = salario;
            return this;
        }

        public Vendedor Construir()
        {
            var salarioDecimal = decimal.Parse(salario, CultureInfo.InvariantCulture);
            return new Vendedor(cpf, nome, salarioDecimal);
        }
    }
}

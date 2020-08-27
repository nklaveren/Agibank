using System;

namespace Agibank.Domain.Entities
{
    public struct Vendedor
    {
        public Vendedor(string cpf, string nome, decimal Salario)
        {
            this.Cpf = cpf;
            this.Nome = nome;
            this.Salario = Salario;
        }

        public string Cpf { get; set; }
        public string Nome { get; set; }
        public decimal Salario { get; set; }
    }
}

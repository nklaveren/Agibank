using Agibank.Domain.Entities;

namespace Agibank.Domain.Builders
{
    public class ClienteBuilder
    {
        string cnpj, nome, areaNegocio;
        public ClienteBuilder ComCnpj(string cnpj)
        {
            this.cnpj = cnpj;
            return this;
        }

        public ClienteBuilder ComNome(string nome)
        {
            this.nome = nome;
            return this;
        }
        public ClienteBuilder ComAreaNegocio(string areaNegocio)
        {
            this.areaNegocio = areaNegocio;
            return this;
        }

        public Cliente Construir()
        {
            return new Cliente(cnpj, nome, areaNegocio);
        }
    }
}

namespace Agibank.Domain.Entities
{
    public struct Cliente
    {
        public Cliente(string cnpj, string name, string areaNegocio)
        {
            this.Cnpj = cnpj;
            this.Nome = name;
            this.AreaNegocio = areaNegocio;
        }

        public string Cnpj { get; set; }
        public string Nome { get; set; }
        public string AreaNegocio { get; set; }
    }
}

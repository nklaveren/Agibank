namespace Agibank.Domain.Entities
{
    public class AnaliseVendasRelatorio
    {
        const string SEPARADOR = "ç";

        public string PiorVendedor { get; set; }
        public long MelhorVenda { get; set; }
        public int Vendedores { get; set; }
        public int Clientes { get; set; }

        public override string ToString()
        {
            return $"{Clientes}{SEPARADOR}{Vendedores}{SEPARADOR}{MelhorVenda}{SEPARADOR}{PiorVendedor}";
        }

    }
}

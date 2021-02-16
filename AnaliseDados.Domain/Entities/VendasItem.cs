namespace AnaliseDados.Domain.Entities
{
    public struct VendasItem
    {

        public VendasItem(int id, long quantidade, decimal preco)
        {
            this.Id = id;
            this.Quantidade = quantidade;
            this.Preco = preco;
            Total = this.Quantidade * Preco;
        }

        public int Id { get; set; }
        public long Quantidade { get; set; }
        public decimal Preco { get; set; }
        public decimal Total { get; }
    }
}

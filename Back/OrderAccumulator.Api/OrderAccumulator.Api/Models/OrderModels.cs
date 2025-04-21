namespace OrderAccumulator.Api.Models
{
    public class OrderModels
    {
        public string Ativo { get; set; } = string.Empty;  // PETR4, VALE3, VIIA4
        public char Lado { get; set; }     // 'C' (Compra) ou 'V' (Venda)
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}

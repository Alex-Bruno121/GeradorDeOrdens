using System.Text.Json.Serialization;

namespace OrderAccumulator.Api.Responses
{
    public class MovimentacoesResponse
    {
        [JsonPropertyName("ativo")]
        public string Ativo { get; set; }

        [JsonPropertyName("lado")]
        public string Lado { get; set; }

        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

        [JsonPropertyName("preco")]
        public decimal Preco { get; set; }

        [JsonPropertyName("exposicao_atual")]
        public decimal ExposicaoAtual { get; set; }

        [JsonPropertyName("data_criacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("status")]
        public int OrdemStatus { get; set; } // 0 - rejeitado | 1 - aceito
    }
}

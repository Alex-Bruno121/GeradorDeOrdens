using System.Text.Json.Serialization;

namespace OrderAccumulator.Api.Responses
{
    public class OrderResponse
    {
        [JsonPropertyName("sucesso")]
        public bool Sucesso { get; set; }
        [JsonPropertyName("exposicao_atual")]
        public decimal Exposicao_Atual { get; set; }
        [JsonPropertyName("msg_erro")]
        public string? Msg_Erro { get; set; }
    }
}

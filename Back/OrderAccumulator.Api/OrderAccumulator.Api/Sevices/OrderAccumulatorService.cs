using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Repositories.Interfaces;
using OrderAccumulator.Api.Responses;
using OrderAccumulator.Api.Services.Interfaces;

namespace OrderAccumulator.Api.Services
{
    public class OrderAccumulatorService : IOrderAccumulatorService
    {
        private readonly IOrderAccumulatorRepository _orderRepository;
        private const decimal LimiteExposicao = 1_000_000m;

        public OrderAccumulatorService(IOrderAccumulatorRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponse> ProcessarOrdemAsync(OrderModels order)
        {
            try
            {
                if (order.Quantidade <= 0 || order.Preco <= 0)
                {
                    return new OrderResponse
                    {
                        Sucesso = false,
                        Msg_Erro = "Quantidade e preço devem ser maiores que zero"
                    };
                }

                decimal valorOrdem = order.Preco * order.Quantidade;
                decimal exposicaoAtual = await _orderRepository.BuscaUltimaExposicaoPorAtivoAsync(order);
                decimal novaExposicao = exposicaoAtual + (order.Lado == 'C' ? valorOrdem : -valorOrdem);

                if (Math.Abs(novaExposicao) > LimiteExposicao)
                {
                    int rs = await _orderRepository.InserirOrdemAsync(order, novaExposicao, 0);
                    return new OrderResponse
                    {
                        Sucesso = false,
                        Msg_Erro = $"Ordem rejeitada - A soma dos ativos ultrapassaria o limite de R$ {LimiteExposicao:N2}"
                    };
                }

                int id = await _orderRepository.InserirOrdemAsync(order, novaExposicao, 1);

                return new OrderResponse
                {
                    Sucesso = true,
                    Exposicao_Atual = novaExposicao
                };
            }
            catch (Exception ex)
            {
                return new OrderResponse
                {
                    Sucesso = false,
                    Msg_Erro = $"Erro ao processar ordem: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<MovimentacoesResponse>> ObterTodasOrdensAsync()
        {
            return await _orderRepository.ObterTodasOrdensAsync();
        }

        public async Task<IEnumerable<MovimentacoesResponse>> ObterUltimasOrdensPorAtivoAsync()
        {
            return await _orderRepository.ObterUltimasOrdensPorAtivoAsync();
        }
    }
}

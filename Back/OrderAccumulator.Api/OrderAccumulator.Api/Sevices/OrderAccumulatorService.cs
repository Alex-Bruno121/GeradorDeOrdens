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

                if ((exposicaoAtual == 0 && order.Lado == 'V') || (valorOrdem > exposicaoAtual && order.Lado == 'V'))
                {
                    await inserirOrdens(order, novaExposicao, 0, "Saldo de ativos insuficiente");
                    return new OrderResponse
                    {
                        Sucesso = false,
                        Msg_Erro = $"Ordem rejeitada - Não é possível realizar uma operação de venda menor que o saldo da sua carteira: R$ {exposicaoAtual:N2}"
                    };
                }

                if (Math.Abs(novaExposicao) > LimiteExposicao)
                {
                    await inserirOrdens(order, novaExposicao, 0, "Limite de exposição maior que 1.000.000,00");
                    return new OrderResponse
                    {
                        Sucesso = false,
                        Msg_Erro = $"Ordem rejeitada - A soma dos ativos ultrapassaria o limite de R$ {LimiteExposicao:N2}"
                    };
                }

                await inserirOrdens(order, novaExposicao, 1);

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

        public async Task inserirOrdens(OrderModels order, decimal exposicao = 0, int status = 0, string motivo = null)
        {
            int qtdIsert = await _orderRepository.InserirOrdemAsync(order, exposicao, status, motivo);
            if (qtdIsert == 0) throw new ArgumentException("Houve um erro ao processar a ordem");
        }

        public async Task<IEnumerable<MovimentacoesResponse>> ObterTodasOrdensAsync()
        {
            return await _orderRepository.ObterTodasOrdensAsync();
        }

        public async Task<IEnumerable<MovimentacoesResponse>> ObterUltimasOrdensPorAtivoAsync()
        {
            return await _orderRepository.ObterUltimasOrdensPorAtivoAsync();
        }

        public async Task LimpaHistoricoAsync()
        {
            await _orderRepository.LimpaHistoricoAsync();
        }
    }
}

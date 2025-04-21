using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Responses;

namespace OrderAccumulator.Api.Services.Interfaces
{
    public interface IOrderAccumulatorService
    {
        Task<OrderResponse> ProcessarOrdemAsync(OrderModels order);
        Task<IEnumerable<MovimentacoesResponse>> ObterTodasOrdensAsync();
        Task<IEnumerable<MovimentacoesResponse>> ObterUltimasOrdensPorAtivoAsync();
    }
}

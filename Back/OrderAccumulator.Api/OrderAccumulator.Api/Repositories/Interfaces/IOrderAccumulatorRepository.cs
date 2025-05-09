﻿using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Responses;

namespace OrderAccumulator.Api.Repositories.Interfaces
{
    public interface IOrderAccumulatorRepository
    {
        Task<decimal> BuscaUltimaExposicaoPorAtivoAsync(OrderModels order);
        Task InserirOrdemAsync(OrderModels order, decimal exposicaoAtual = 0, int status = 0, string motivo = null);
        Task LimpaHistoricoAsync();
        Task<IEnumerable<MovimentacoesResponse>> ObterTodasOrdensAsync();
        Task<IEnumerable<MovimentacoesResponse>> ObterUltimasOrdensPorAtivoAsync();
    }
}
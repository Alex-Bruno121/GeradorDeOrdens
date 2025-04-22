using System.Data;
using Dapper;
using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Repositories.Interfaces;
using OrderAccumulator.Api.Responses;
using OrderAccumulator.Api.Scripts;

namespace OrderAccumulator.Api.Repositories
{
    public class OrderAccumulatorRepository : IOrderAccumulatorRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly OrderScript _orderScript;

        public OrderAccumulatorRepository(IDbConnection dbConnection, OrderScript orderScript)
        {
            _dbConnection = dbConnection;
            _orderScript = orderScript;
        }

        public async Task<decimal> BuscaUltimaExposicaoPorAtivoAsync(OrderModels order)
        {
            DynamicParameters d = new DynamicParameters();
            d.Add("ATIVO", order.Ativo);

            return await _dbConnection.QueryFirstOrDefaultAsync<decimal>(_orderScript.BuscaUltimaExposicaoPorAtivo, d);
        }

        public async Task<int> InserirOrdemAsync(OrderModels order, decimal exposicaoAtual = 0, int status = 0, string motivo = null)
        {
            DynamicParameters d = new DynamicParameters();
            d.Add("ATIVO", order.Ativo);
            d.Add("LADO", order.Lado);
            d.Add("QUANTIDADE", order.Quantidade);
            d.Add("PRECO", order.Preco);
            d.Add("EXPOSICAO_ATUAL", exposicaoAtual);
            d.Add("ORDEM_STATUS", status);
            d.Add("MOTIVO", motivo);
            d.Add("DATA", DateTime.Now);

            return await _dbConnection.ExecuteAsync(_orderScript.InserirOrdem, d);
        }

        public async Task LimpaHistoricoAsync()
        {
            await _dbConnection.ExecuteAsync(_orderScript.LimpaTabela);
        }

        public async Task<IEnumerable<MovimentacoesResponse>> ObterTodasOrdensAsync()
        {
            return await _dbConnection.QueryAsync<MovimentacoesResponse>(_orderScript.ObterTodasOrdens);
        }

        public async Task<IEnumerable<MovimentacoesResponse>> ObterUltimasOrdensPorAtivoAsync()
        {
            return await _dbConnection.QueryAsync<MovimentacoesResponse>(_orderScript.UltimasOrdensPorAtivo);
        }
    }
}

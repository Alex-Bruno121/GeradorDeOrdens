using Microsoft.AspNetCore.Mvc;
using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Services.Interfaces;

namespace OrderAccumulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderAccumulatorService _orderDbService;

        public OrderController(IOrderAccumulatorService orderDbService)
        {
            _orderDbService = orderDbService;
        }

        /// <summary>
        /// Processa ordens de compra e venda de ativos
        /// </summary>
        /// <param name="order">Dados da ordem (ativo, lado, quantidade, preço)</param>
        /// <returns>Resultado do processamento com exposição atual</returns>
        /// <response code="200">Retorna um objeto json (sucesso, exposicao_atual e msg_erro)</response>
        [HttpPost("processar/ordem-ativo")]
        public async Task<IActionResult> ProcessarOrdem([FromBody] OrderModels order)
        {
            return Ok(await _orderDbService.ProcessarOrdemAsync(order));
        }

        /// <summary>
        /// Resgata todo o historico de ordens de compra e venda de ativos
        /// </summary>
        /// <returns>Retorna uma lista de movimentações</returns>
        /// <response code="200">Retorna uma lista de movimentações (MovimentacoesResponse)</response>
        [HttpGet("busca/historicos-movimentacoes")]
        public async Task<IActionResult> ObterTodasOrdens()
        {
            var movimentacoes = await _orderDbService.ObterTodasOrdensAsync();
            if (movimentacoes == null || !movimentacoes.Any())
                return NoContent();

            return Ok(movimentacoes);
        }

        /// <summary>
        /// Resgata as ultimas ordens de compra e venda por ativos
        /// </summary>
        /// <returns>Retorna uma lista das ultimas movimentações por ativos</returns>
        /// <response code="200">Retorna uma lista das ultimas movimentações por ativos (MovimentacoesResponse)</response>
        [HttpGet("busca/ultimas-movimentacoes")]
        public async Task<IActionResult> ObterUltimasOrdensPorAtivo()
        {
            var movimentacoes = await _orderDbService.ObterUltimasOrdensPorAtivoAsync();
            if (movimentacoes == null || !movimentacoes.Any())
                return NoContent();

            return Ok(movimentacoes);
        }

        /// <summary>
        /// Limpa a tabela de historico de ordens para reprocessamento (apenas para novos testes)
        /// </summary>
        /// <response code="204">Retorna noContent (204)</response>
        [HttpDelete("limpa-historico")]
        public async Task<IActionResult> LimpaHistorico()
        {
            await _orderDbService.LimpaHistoricoAsync();
            return NoContent();
        }
    }
}

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

        [HttpPost("processar/ordem-ativo")]
        public async Task<IActionResult> PostProcessarOrdem([FromBody] OrderModels order)
        {
            var response = await _orderDbService.ProcessarOrdemAsync(order);

            if (response.Sucesso)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpGet("busca/historicos-movimentacoes")]
        public async Task<IActionResult> GetObterTodasOrdens()
        {
            var movimentacoes = await _orderDbService.ObterTodasOrdensAsync();
            return Ok(movimentacoes);
        }

        [HttpGet("busca/ultimas-movimentacoes")]
        public async Task<IActionResult> GetObterUltimasOrdensPorAtivo()
        {
            var movimentacoes = await _orderDbService.ObterUltimasOrdensPorAtivoAsync();
            return Ok(movimentacoes);
        }
    }
}

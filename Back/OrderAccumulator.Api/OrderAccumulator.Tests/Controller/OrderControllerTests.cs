using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderAccumulator.Api.Controllers;
using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Responses;
using OrderAccumulator.Api.Services.Interfaces;
using Xunit;

namespace OrderAccumulator.Tests.Controller
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderAccumulatorService> _serviceMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _serviceMock = new Mock<IOrderAccumulatorService>();
            _controller = new OrderController(_serviceMock.Object);
        }

        [Fact]
        public async Task ProcessarOrdem_QuandoOrdemValida_DeveRetornarOk()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'C',
                Quantidade = 100,
                Preco = 28.50m
            };

            var response = new OrderResponse
            {
                Sucesso = true,
                Exposicao_Atual = 100,
                Msg_Erro = null
            };

            _serviceMock.Setup(s => s.ProcessarOrdemAsync(It.IsAny<OrderModels>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.ProcessarOrdem(ordem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderResponse>(okResult.Value);
            Assert.True(returnValue.Sucesso);
        }

        [Fact]
        public async Task ProcessarOrdem_QuandoOrdemInvalida_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "INVALID",
                Lado = 'X',
                Quantidade = -1,
                Preco = 0
            };

            var response = new OrderResponse
            {
                Sucesso = false,
                Exposicao_Atual = 0,
                Msg_Erro = "Ordem inválida"
            };

            _serviceMock.Setup(s => s.ProcessarOrdemAsync(It.IsAny<OrderModels>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _controller.ProcessarOrdem(ordem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderResponse>(okResult.Value);
            Assert.False(returnValue.Sucesso);
            Assert.NotNull(returnValue.Msg_Erro);
        }

        [Fact]
        public async Task ObterTodasOrdens_QuandoExistemOrdens_DeveRetornarLista()
        {
            // Arrange
            var movimentacoes = new List<MovimentacoesResponse>
            {
                new MovimentacoesResponse
                {
                    Ativo = "PETR4",
                    Lado = "C",
                    Quantidade = 100,
                    Preco = 28.50m,
                    ExposicaoAtual = 100,
                    DataCriacao = DateTime.Now,
                    OrdemStatus = 1
                }
            };

            _serviceMock.Setup(s => s.ObterTodasOrdensAsync())
                       .ReturnsAsync(movimentacoes);

            // Act
            var result = await _controller.ObterTodasOrdens();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<MovimentacoesResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task ObterTodasOrdens_QuandoNaoExistemOrdens_DeveRetornarNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.ObterTodasOrdensAsync())
                       .ReturnsAsync(new List<MovimentacoesResponse>());

            // Act
            var result = await _controller.ObterTodasOrdens();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ObterUltimasOrdensPorAtivo_QuandoExistemOrdens_DeveRetornarLista()
        {
            // Arrange
            var movimentacoes = new List<MovimentacoesResponse>
            {
                new MovimentacoesResponse
                {
                    Ativo = "VALE3",
                    Lado = "V",
                    Quantidade = 50,
                    Preco = 72.30m,
                    ExposicaoAtual = -50,
                    DataCriacao = DateTime.Now,
                    OrdemStatus = 1
                }
            };

            _serviceMock.Setup(s => s.ObterUltimasOrdensPorAtivoAsync())
                       .ReturnsAsync(movimentacoes);

            // Act
            var result = await _controller.ObterUltimasOrdensPorAtivo();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<MovimentacoesResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task ObterUltimasOrdensPorAtivo_QuandoNaoExistemOrdens_DeveRetornarNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.ObterUltimasOrdensPorAtivoAsync())
                       .ReturnsAsync(new List<MovimentacoesResponse>());

            // Act
            var result = await _controller.ObterUltimasOrdensPorAtivo();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task LimpaHistorico_QuandoExecutadoComSucesso_DeveRetornarNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.LimpaHistoricoAsync())
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.LimpaHistorico();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task LimpaHistorico_QuandoOcorreErro_DevePropagar()
        {
            // Arrange
            _serviceMock.Setup(s => s.LimpaHistoricoAsync())
                       .ThrowsAsync(new Exception("Erro ao limpar histórico"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.LimpaHistorico());
        }
    }
}

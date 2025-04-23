using Moq;
using OrderAccumulator.Api.Models;
using OrderAccumulator.Api.Repositories.Interfaces;
using OrderAccumulator.Api.Responses;
using OrderAccumulator.Api.Services;
using Xunit;

namespace OrderAccumulator.Tests.Services
{
    public class OrderAccumulatorServiceTests
    {
        private readonly Mock<IOrderAccumulatorRepository> _repositoryMock;
        private readonly OrderAccumulatorService _service;

        public OrderAccumulatorServiceTests()
        {
            _repositoryMock = new Mock<IOrderAccumulatorRepository>();
            _service = new OrderAccumulatorService(_repositoryMock.Object);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoOrdemNula_DeveRetornarErro()
        {
            // Arrange
            OrderModels ordem = null;

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal("Ordem não pode ser nula", resultado.Msg_Erro);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoAtivoVazio_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "",
                Lado = 'C',
                Quantidade = 100,
                Preco = 28.50m
            };

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal("Ativo não pode ser vazio ou ser diferente dos ativos: PETR4, VALE3, VIIA4", resultado.Msg_Erro);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoAtivoErrado_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4z",
                Lado = 'C',
                Quantidade = 100,
                Preco = 28.50m
            };

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal("Ativo não pode ser vazio ou ser diferente dos ativos: PETR4, VALE3, VIIA4", resultado.Msg_Erro);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoLadoInvalido_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'X',
                Quantidade = 100,
                Preco = 28.50m
            };

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal("Lado da ordem deve ser 'C' para compra ou 'V' para venda", resultado.Msg_Erro);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoQuantidadeOuPrecoInvalido_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'C',
                Quantidade = 0,
                Preco = -10.00m
            };

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal("Quantidade e preço devem ser maiores que zero", resultado.Msg_Erro);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoOrdemCompraValida_DeveProcessarComSucesso()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'C',
                Quantidade = 100,
                Preco = 28.50m
            };

            _repositoryMock.Setup(r => r.BuscaUltimaExposicaoPorAtivoAsync(It.IsAny<OrderModels>()))
                          .ReturnsAsync(0m);

            _repositoryMock.Setup(r => r.InserirOrdemAsync(It.IsAny<OrderModels>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>()));

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal(2850m, resultado.Exposicao_Atual); // 100 * 28.50
            Assert.Null(resultado.Msg_Erro);
            _repositoryMock.Verify(r => r.InserirOrdemAsync(ordem, 2850m, 1, null), Times.Once);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoUltrapassaLimiteExposicao_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'C',
                Quantidade = 50000,
                Preco = 30.00m
            };

            _repositoryMock.Setup(r => r.BuscaUltimaExposicaoPorAtivoAsync(It.IsAny<OrderModels>()))
                          .ReturnsAsync(500000m);

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains("limite de R$ 1.000.000,00", resultado.Msg_Erro);
            _repositoryMock.Verify(r => r.InserirOrdemAsync(It.IsAny<OrderModels>(), It.IsAny<decimal>(), 0, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoTentaVenderSemSaldo_DeveRetornarErro()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'V',
                Quantidade = 100,
                Preco = 28.50m
            };

            _repositoryMock.Setup(r => r.BuscaUltimaExposicaoPorAtivoAsync(It.IsAny<OrderModels>()))
                          .ReturnsAsync(0m);

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains("saldo insuficiente", resultado.Msg_Erro);
            _repositoryMock.Verify(r => r.InserirOrdemAsync(It.IsAny<OrderModels>(), 0m, 0, "Saldo de ativos insuficiente"), Times.Once);
        }

        [Fact]
        public async Task ObterTodasOrdensAsync_QuandoExistemOrdens_DeveRetornarLista()
        {
            // Arrange
            var ordensEsperadas = new List<MovimentacoesResponse>
            {
                new MovimentacoesResponse
                {
                    Ativo = "PETR4",
                    Lado = "C",
                    Quantidade = 100,
                    Preco = 28.50m,
                    ExposicaoAtual = 2850m,
                    DataCriacao = DateTime.Now,
                    OrdemStatus = 1
                }
            };

            _repositoryMock.Setup(r => r.ObterTodasOrdensAsync())
                          .ReturnsAsync(ordensEsperadas);

            // Act
            var resultado = await _service.ObterTodasOrdensAsync();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(ordensEsperadas, resultado);
        }

        [Fact]
        public async Task ObterTodasOrdensAsync_QuandoNaoExistemOrdens_DeveRetornarListaVazia()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ObterTodasOrdensAsync())
                          .ReturnsAsync(new List<MovimentacoesResponse>());

            // Act
            var resultado = await _service.ObterTodasOrdensAsync();

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterUltimasOrdensPorAtivoAsync_QuandoExistemOrdens_DeveRetornarLista()
        {
            // Arrange
            var ordensEsperadas = new List<MovimentacoesResponse>
            {
                new MovimentacoesResponse
                {
                    Ativo = "VALE3",
                    Lado = "V",
                    Quantidade = 50,
                    Preco = 72.30m,
                    ExposicaoAtual = -3615m,
                    DataCriacao = DateTime.Now,
                    OrdemStatus = 1
                }
            };

            _repositoryMock.Setup(r => r.ObterUltimasOrdensPorAtivoAsync())
                          .ReturnsAsync(ordensEsperadas);

            // Act
            var resultado = await _service.ObterUltimasOrdensPorAtivoAsync();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(ordensEsperadas, resultado);
        }

        [Fact]
        public async Task LimpaHistoricoAsync_QuandoExecutadoComSucesso_NaoDeveGerarErro()
        {
            // Arrange
            _repositoryMock.Setup(r => r.LimpaHistoricoAsync())
                          .Returns(Task.CompletedTask);

            // Act & Assert
            await _service.LimpaHistoricoAsync();
            _repositoryMock.Verify(r => r.LimpaHistoricoAsync(), Times.Once);
        }

        [Fact]
        public async Task LimpaHistoricoAsync_QuandoOcorreErro_DevePropagar()
        {
            // Arrange
            _repositoryMock.Setup(r => r.LimpaHistoricoAsync())
                          .ThrowsAsync(new Exception("Erro ao limpar histórico"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.LimpaHistoricoAsync());
        }

        [Fact]
        public async Task ProcessarOrdemAsync_QuandoRepositorioLancaExcecao_DeveRetornarErroTratado()
        {
            // Arrange
            var ordem = new OrderModels
            {
                Ativo = "PETR4",
                Lado = 'C',
                Quantidade = 100,
                Preco = 28.50m
            };

            _repositoryMock.Setup(r => r.BuscaUltimaExposicaoPorAtivoAsync(It.IsAny<OrderModels>()))
                           .ThrowsAsync(new Exception("Erro de conexão com banco de dados"));

            // Act
            var resultado = await _service.ProcessarOrdemAsync(ordem);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains("Erro ao processar ordem:", resultado.Msg_Erro);
            Assert.Contains("Erro de conexão com banco de dados", resultado.Msg_Erro);
            Assert.Equal(0m, resultado.Exposicao_Atual);

            // Verifica que nenhuma ordem foi inserida
            _repositoryMock.Verify(r => r.InserirOrdemAsync(
                It.IsAny<OrderModels>(),
                It.IsAny<decimal>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            ), Times.Never);
        }
    }
}

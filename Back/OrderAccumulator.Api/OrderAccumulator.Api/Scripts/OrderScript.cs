namespace OrderAccumulator.Api.Scripts
{
    public class OrderScript
    {
        public string UltimasOrdensPorAtivo = @"
            WITH ULTIMASTRANSACOES AS (
                SELECT 
                    ATIVO,
                    LADO,
                    QUANTIDADE,
                    PRECO,
                    EXPOSICAO_ATUAL,
                    ORDEM_STATUS,
            		DATA_CRIACAO,
                    ROW_NUMBER() OVER (PARTITION BY ATIVO ORDER BY DATA_CRIACAO DESC) AS RANKING
                FROM 
                    ORDERS
                WHERE 
                    ATIVO IN ('PETR4', 'VALE3', 'VIIA4') AND ORDEM_STATUS = 1
            		
            )
            SELECT 
                ATIVO as Ativo,
                LADO as Lado,
                QUANTIDADE as Quantidade,
                PRECO as Preco,
                EXPOSICAO_ATUAL as ExposicaoAtual,
                ORDEM_STATUS as OrdemStatus,
            	DATA_CRIACAO as DataCriacao
            FROM 
                ULTIMASTRANSACOES
            WHERE 
                RANKING = 1
            ORDER BY 
                ATIVO";

        public string ObterTodasOrdens = @"
            SELECT 
                ATIVO as Ativo,
                LADO as Lado,
                QUANTIDADE as Quantidade,
                PRECO as Preco,
                EXPOSICAO_ATUAL as ExposicaoAtual,
                ORDEM_STATUS as OrdemStatus,
            	DATA_CRIACAO as DataCriacao
            FROM 
                ORDERS
            ORDER BY 
                DATACRIACAO DESC";

        public string BuscaUltimaExposicaoPorAtivo = @"
            SELECT TOP 1 EXPOSICAO_ATUAL as ExposicaoAtual 
            FROM ORDERS 
            WHERE ATIVO = @ATIVO 
            ORDER BY DATA_CRIACAO DESC";

        public string InserirOrdem = @"
            INSERT INTO ORDERS (ATIVO, LADO, QUANTIDADE, PRECO, EXPOSICAO_ATUAL, ORDEM_STATUS)
            VALUES (@ATIVO, @LADO, @QUANTIDADE, @PRECO, @EXPOSICAO_ATUAL, @ORDEM_STATUS);
            SELECT CAST(SCOPE_IDENTITY() as int);";
    }
}

# Documentação Order Accumulator API

## Visão Geral
API para gerenciamento e processamento de ordens de compra e venda de ativos financeiros, com controle de exposição e histórico de movimentações.

## Endpoints

### 1. Processamento de Ordens
**Endpoint:** `POST /Order/processar/ordem-ativo`

**Descrição:**  
Processa ordens de compra e venda de ativos financeiros.

**Request:**
```json
{   
    "ativo": "string",     // PETR4, VALE3 ou VIIA4 
    "lado": "char",        // C (Compra) ou V (Venda) 
    "quantidade": "int",   // Quantidade de ativos 
    "preco": "decimal"     // Preço unitário do ativo 
}
```

**Response (200 OK):**
```json
{ 
    "sucesso": true, 
    "exposicao_atual": 0.0, 
    "msg_erro": null 
}
```

**Response (200 OK):**
```json
{ 
    "sucesso": false, 
    "exposicao_atual": 0.0, 
    "msg_erro": "Ordem rejeitada - Não é possível realizar ..." 
}
```
**OBS**: *Quando a operação retorna false por motivos de regra de negócio (Atingiu o limite ou não tem saldo no ativo selecionado)*

**Regras de Negócio:**
- Validações:
  - Ativo deve ser um dos seguintes: PETR4, VALE3 ou VIIA4
  - Lado deve ser 'C' para Compra ou 'V' para Venda
  - Quantidade e preço devem ser valores positivos
- Controle de exposição é calculado por ativo
- Status da ordem:
  - 1 = Aceita
  - 0 = Rejeitada (com motivo detalhado)

### 2. Consulta de Histórico Completo
**Endpoint:** `GET /Order/busca/historicos-movimentacoes`

**Descrição:**  
Retorna todo o histórico de ordens processadas.

**Response (200 OK):**
```json
[{ 
    "ativo": "string", 
    "lado": "string", 
    "quantidade": 0, 
    "preco": 0.0, 
    "exposicao_atual": 0.0, 
    "data_criacao": "datetime", 
    "status": 0, 
    "motivo": "string" 
}]
```

**Response (204 No Content):**  
Retornado quando não existem registros.

### 3. Consulta das Últimas Movimentações
**Endpoint:** `GET /Order/busca/ultimas-movimentacoes`

**Descrição:**  
Retorna a última ordem processada para cada ativo.

**Response (200 OK):**
```json
[{ 
    "ativo": "string", 
    "lado": "string", 
    "quantidade": 0, 
    "preco": 0.0, 
    "exposicao_atual": 0.0, 
    "data_criacao": "datetime", 
    "status": 0, 
    "motivo": "string" 
}]
```

**Response (204 No Content):**  
Retornado quando não existem registros.

**Regras de Negócio:**
- Retorna apenas ordens com status = 1 (aceitas)
- Uma ordem por ativo (a mais recente)
- Ordenação por data de criação decrescente

### 4. Limpeza de Histórico
**Endpoint:** `DELETE /Order/limpa-historico`

**Descrição:**  
Remove todos os registros do histórico de ordens (*Essa rota existe apenas para fins de teste da aplicação*).

**Response:** `204 No Content`

**Regras de Negócio:**
- Remove todas as ordens do banco de dados
- Operação irreversível
- Utilizado para reinicializar o sistema

## Modelos de Dados

### OrderModels
```csharp
public class OrderModels 
{ 
    public string Ativo { get; set; }      // PETR4, VALE3, VIIA4 
    public char Lado { get; set; }         // 'C' ou 'V' 
    public int Quantidade { get; set; } 
    public decimal Preco { get; set; } 
}
```
### OrderResponse
```csharp
public class OrderResponse
{
    public bool Sucesso { get; set; }
    public decimal Exposicao_Atual { get; set; }
    public string? Msg_Erro { get; set; }
}
```

### MovimentacoesResponse
```csharp
public class MovimentacoesResponse
{ 
    public string Ativo { get; set; } 
    public string Lado { get; set; } 
    public int Quantidade { get; set; } 
    public decimal Preco { get; set; } 
    public decimal ExposicaoAtual { get; set; } 
    public DateTime DataCriacao { get; set; } 
    public int OrdemStatus { get; set; }            // 0=rejeitado, 1=aceito 
    public string? Motivo { get; set; } 
}
```

## Banco de Dados
A API utiliza uma tabela `ORDERS` para persistência dos dados com os seguintes campos:
- ATIVO
- LADO
- QUANTIDADE
- PRECO
- EXPOSICAO_ATUAL
- ORDEM_STATUS
- MOTIVO
- DATA_CRIACAO

*OBS:* **Caso o banco de dados não exista, utilizar o script abaixo para cria-lo**

```sql
CREATE TABLE ORDERS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ATIVO NVARCHAR(50) NOT NULL,
    LADO CHAR(1) NOT NULL, -- 'C' PARA COMPRA, 'V' PARA VENDA
    QUANTIDADE INT NOT NULL,
    PRECO DECIMAL(18, 2) NOT NULL,
    EXPOSICAO_ATUAL DECIMAL(18, 2)NOT NULL,
    ORDEM_STATUS BIT NOT NULL DEFAULT 1,
    MOTIVO NVARCHAR(50) NULL,
    DATA_CRIACAO DATETIME NOT NULL
);
```
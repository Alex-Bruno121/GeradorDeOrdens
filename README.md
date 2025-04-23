# Sistema Acumulador de Pedidos

Este projeto consiste em uma API back-end para gerenciamento de pedidos e uma interface front-end para geração de pedidos.

## Back-end (OrderAccumulator.Api)

### Arquitetura
- Construído com ASP.NET Core 8.0
- Segue princípios de Arquitetura Limpa (Clean Architecture) com separação de responsabilidades:
  - Controllers: Gerenciam requisições/respostas HTTP
  - Services: Contêm a lógica de negócio
  - Repositories: Gerencia o acesso ao banco de dados.
  - Scripts: Contém scripts SQL utilizados no repositório.
  - Models: Definem estruturas de dados
- Utiliza Injeção de Dependência para baixo acoplamento
- Inclui testes unitários usando xUnit e Moq

**Para maior entendimento do projeto, acessar a documentação na pagina do git a seguir: /Doc e navegar entre Api.md e Front.md**

### Banco de dados - SQL Server.
- *O banco de dados utilizado está em um servidor da https://www.smarterasp.net/ com uso grátis de 60 dias.*

### Funcionalidades
-	Processar Ordens: Processa ordens de compra e venda de ativos, validando limites de exposição e saldo.
-	Consultar Histórico: Retorna todas as movimentações de ordens registradas.
-	Consultar Últimas Movimentações: Retorna as últimas ordens de compra e venda por ativo.
-	Limpar Histórico: Remove todas as ordens registradas para reprocessamento.

### Como Executar
1. Navegue até o diretório `Back/OrderAccumulator.Api`
2. Abra a solução do projeto
3. Build a aplicação
4. Execute o projeto.

A API iniciará em https://localhost:7019

## Front-end (order-generator)

### Arquitetura
- Construído com React + TypeScript + Vite
- Utiliza Ant Design (antd) para componentes de UI
- Gerenciamento de estado com React hooks
- Estrutura modular de componentes:
  - Pages: Componentes principais de visualização
  - Components: Elementos de UI reutilizáveis
  - Types: Definições TypeScript
  - Utils: Funções auxiliares

### Como Executar
1. Navegue até o diretório Front/order-generator
2. Utilizar o nodejs na versão 22.14.0
3. Instale as dependências:

```sh
npm install
```

3. Inicie o servidor de desenvolvimento:

```sh
npm run dev
```

A aplicação estará disponível em http://localhost:8080
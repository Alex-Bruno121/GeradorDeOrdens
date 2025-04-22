# Order Generator - Documentação

## 🎯 Visão Geral
O Order Generator é uma aplicação React/TypeScript desenvolvida para gerenciamento de ordens de compra e venda de ativos financeiros, oferecendo uma interface intuitiva e funcional.

### Formulário Principal

#### 1. Seleção de Ativo
- **Tipo**: Dropdown
- **Opções**: PETR4, VALE3, VIIA4
- **Validação**: Campo obrigatório

#### 2. Lado da Operação
- **Tipo**: Botões de rádio
- **Opções**:
  - Compra (C) - Verde
  - Venda (V) - Vermelho
- **Visual**: Ícones de seta para cada operação

#### 3. Campo de Quantidade
- **Tipo**: Input numérico
- **Restrições**:
  - Valor mínimo: 1
  - Valor máximo: 99.999
  - Apenas números inteiros

#### 4. Campo de Preço
- **Tipo**: Input numérico
- **Restrições**:
  - Valor mínimo: 0,01
  - Valor máximo: 999,99
  - Até 2 casas decimais

### Card de Movimentações
#### Botões de Ação
- **Histórico** (🕒)
  - Abre modal com histórico completo
- **Deletar** (🗑️)
  - Limpa histórico de movimentações

### Histórico de Movimentações
- Visualização em tempo real
- Modal com histórico completo
- Diferenciação visual entre ordens:
  - ✅ Processadas
  - ❌ Rejeitadas

## 📝 Guia de Uso

1. **Iniciar Nova Ordem**:
   - Selecione o ativo
   - Escolha o lado (Compra/Venda)
   - Digite quantidade
   - Insira preço (Utilizar valores com duas casas decimais)

**Tela inicial**
<img src="/Front/order-generator/public/images/page-principal.png" alt="Pagina principal" />

**Enviando ordem**
<img src="/Front/order-generator/public/images/page-enviando-order-sucesso.png" alt="Enviando ordem" />

**Retornando mensagem de sucesso + carregamento de historico de ordens por ativo**
<img src="/Front/order-generator/public/images/page-card-movimentacoes.png" alt="Sucesso ao processar ordem" />

**Tela com retorno de erro de limite**
<img src="/Front/order-generator/public/images/ordem-rejeitada.png" alt="Erro ao processar ordem" />

**Tela de historico de movimentações (Mostra movimentos de ativo de compra e venda com seus status de processada ou rejeitada)**
<img src="/Front/order-generator/public/images/historico-erros-sucessos.png" alt="Historico de ordens" />

**Na pagina, existe uma validação para não permitir o usuário enviar requisição para a api sem preenchimento completo ou parcial**
<img src="/Front/order-generator/public/images/page-principal-validacoes.png" alt="Validação de formulários" />

**Foi predefinido uma regra de negócio, que não é permitido vender um ativo quando seu valor da ordem (preco * qtd) for maior que a exposição atual**
<img src="/Front/order-generator/public/images/saldo-insuficiente.png" alt="Saldo insuficiente" />

2. **Acompanhar Movimentações**:
   - Use o botão de histórico para mais detalhes
   - A tela consome a api para obter as ultimas movimentações por ativo

<img src="/Front/order-generator/public/images/card-historico-ativos.png" alt="Historico por ativos" />

3. **Gerenciar Histórico**:
   - Visualize todas as operações no modal
   - Utilize o botão deletar para limpar quando necessário

<img src="/Front/order-generator/public/images/historico-erros-sucessos.png" alt="Historico de ordens" />

<img src="/Front/order-generator/public/images/Limpar-historico.png" alt="Limpa ordens" />
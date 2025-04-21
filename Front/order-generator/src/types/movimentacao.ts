export interface Movimentacao {
  ativo: string;
  lado: 'C' | 'V';
  quantidade: number;
  preco: number;
  exposicao_atual: number;
  data_criacao: string;
  status: number;
  motivo: string | null;
}

export interface MovimentacaoResponse {
  exposicoes: Movimentacao[];
}
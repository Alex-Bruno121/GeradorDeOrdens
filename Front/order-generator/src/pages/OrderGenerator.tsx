import React, { useEffect, useState } from 'react';
import { Form, Button, Select, InputNumber, Card, message, Radio } from 'antd';
import { ArrowUpOutlined, ArrowDownOutlined } from '@ant-design/icons';
import MovimentacaoCard from '../components/MovimentacaoCard';
import { Movimentacao } from '../types/movimentacao';
import { OrderFormValues } from '../types/order';
import { orderValidators } from '../utils/validators';

const { Option } = Select;
const base_url = import.meta.env.VITE_API_TODO;
const ativoOptions = ['PETR4', 'VALE3', 'VIIA4'];

const OrderGenerator: React.FC = () => {
  const [form] = Form.useForm();
  const [submitting, setSubmitting] = useState(false);
  const [movimentacao, setMovimentacao] = useState<Movimentacao[]>([]);

  const fetchExposicoes = async () => {
    try {
      
      const response = await fetch(`${base_url}/Order/busca/ultimas-movimentacoes`);
      if (response.status == 204) {
        setMovimentacao([]);
      } else if (response.status == 200) {
        const data = await response.json();
        const mov = data.map((item: Movimentacao) => ({
          ...item,
          preco: new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          }).format(item.preco),
          exposicao_atual: new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          }).format(item.exposicao_atual),
          data_criacao: new Date(item.data_criacao).toLocaleString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
          })
        }));
        setMovimentacao(mov);
      } else {
        message.error('Erro ao carregar movimentações', 3);
      }
    } catch (error) {
      message.error('Erro ao carregar movimentações', 3);
    }
  };

  useEffect(() => {
    fetchExposicoes();
  }, []);

  const onFinish = async (values: OrderFormValues) => {
    setSubmitting(true);
    message.info('Processando ordem', 1.8);
    try {
      const response = await fetch(`${base_url}/Order/processar/ordem-ativo`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(values),
      });

      const data = await response.json();

      if (data.sucesso === false) {
        setTimeout(function () {
          message.error(`${data.msg_erro}`, 8);
        }, 2000)
      } else {
        setTimeout(function () {
          message.success('Ordem processado com sucesso', 4);
        }, 1000)
        form.resetFields();
        await fetchExposicoes();
      }
    }
    catch (error: any) {
      message.error('Houve um erro ao tentar processar sua ordem', 8);
    }
    finally {
      setSubmitting(false);
    }
  };

  return (
    <Card
      style={{ maxWidth: 800, margin: '0 auto' }}
    >
      <Form
        form={form}
        layout="vertical"
        onFinish={onFinish}
        initialValues={{ lado: 'C' }}
      >
        {/* Campo Ativo  */}
        <Form.Item
          name="ativo"
          label="Ativo"
          rules={[{ required: true, message: 'Por favor, selecione o ativo!' }]}
        >
          <Select placeholder="Selecione um ativo">
            {ativoOptions.map(ativo => (
              <Option key={ativo} value={ativo}>
                {ativo}
              </Option>
            ))}
          </Select>
        </Form.Item>

        {/* Campo Lado (Compra/Venda) */}
        <Form.Item name="lado" label="Lado" rules={[{ required: true }]}>
          <Radio.Group buttonStyle="solid">
            <Radio.Button
              value="C"
              className="custom-radio-button"
              style={{ color: '#52c41a' }}
            >
              <ArrowUpOutlined /> Compra
            </Radio.Button>
            <Radio.Button
              value="V"
              className="custom-radio-button"
              style={{ color: '#f5222d' }}
            >
              <ArrowDownOutlined /> Venda
            </Radio.Button>
          </Radio.Group>
        </Form.Item>

        {/* Campo Quantidade */}
        <Form.Item
          name="quantidade"
          label="Quantidade"
          rules={[
            { required: true, message: 'Por favor, informe a quantidade!' },
            { validator: orderValidators.validateQuantidade }
          ]}
        >
          <InputNumber
            min={1}
            max={99999}
            style={{ width: '100%' }}
            placeholder="Quantidade de ativos"
          />
        </Form.Item>

        {/* Campo Preço */}
        <Form.Item
          name="preco"
          label="Preço"
          rules={[
            { required: true, message: 'Por favor, informe o preço!' },
            { validator: orderValidators.validatePreco }
          ]}
        >
          <InputNumber<number>
            min={0.01}
            max={999.99}
            step={0.01}
            style={{ width: '100%' }}
            placeholder="0.01"
            formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
            parser={(value) => {
              if (!value) return 0;
              return value.replace(',', '.').replace(/\$\s?|(,*)/g, '') as unknown as number;
            }}
          />
        </Form.Item>

        {/* Botão de Envio */}
        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            loading={submitting}
            size="large"
            block
            style={{ background: '#2f3a2f' }}
          >
            Enviar Ordem
          </Button>
        </Form.Item>
      </Form>
      {movimentacao.length > 0 && (
        <MovimentacaoCard
          movimentacao={movimentacao}
          onDelete={fetchExposicoes}
        />
      )}
    </Card>
  );
};

export default OrderGenerator;

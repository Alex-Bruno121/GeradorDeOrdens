import React, { useState } from 'react';
import { Button, Card, message, Modal, Popconfirm, Table } from 'antd';
import { Movimentacao } from '../types/movimentacao';
import { DeleteOutlined, HistoryOutlined } from '@ant-design/icons';

interface MovimentacaoCardProps {
    movimentacao: Movimentacao[];
    onDelete: () => void;
}

const MovimentacaoCard: React.FC<MovimentacaoCardProps> = ({ movimentacao, onDelete }) => {
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [historicoMovimentacoes, setHistoricoMovimentacoes] = useState<Movimentacao[]>([]);
    const [loadingHistorico, setLoadingHistorico] = useState(false);

    const columns = [
        {
            title: 'Ativo',
            dataIndex: 'ativo',
            key: 'ativo',
        },
        {
            title: 'Lado',
            dataIndex: 'lado',
            key: 'lado',
        },
        {
            title: 'Quantidade',
            dataIndex: 'quantidade',
            key: 'quantidade',
        },
        {
            title: 'Preço',
            dataIndex: 'preco',
            key: 'preco',
        },
        {
            title: 'Exposição',
            dataIndex: 'exposicao_atual',
            key: 'exposicao_atual',
        },
        {
            title: 'Data Movimento',
            dataIndex: 'data_criacao',
            key: 'data_criacao',
        },
    ];

    const historicoColumns = [
        {
            title: 'Ativo',
            dataIndex: 'ativo',
            key: 'ativo',
        },
        {
            title: 'Lado',
            dataIndex: 'lado',
            key: 'lado',
        },
        {
            title: 'Quantidade',
            dataIndex: 'quantidade',
            key: 'quantidade',
        },
        {
            title: 'Preço',
            dataIndex: 'preco',
            key: 'preco',
        },
        {
            title: 'Status',
            dataIndex: 'statusText',
            key: 'status',
            render: (status: string) => (
                <span>{status}</span>
            )
        },
        {
            title: 'Data Movimento',
            dataIndex: 'data_criacao',
            key: 'data_criacao',
        },
        {
            title: 'Motivo Rejeição',
            dataIndex: 'motivo',
            key: 'motivo',
        },
    ];

    const handleDelete = async () => {
        try {
            const response = await fetch(`${import.meta.env.VITE_API_TODO}/Order/limpa-historico`, {
                method: 'DELETE',
            });
            if (response.status == 204) {
                message.success('Movimentações deletadas com sucesso');
                onDelete();
            } 
        } catch (error) {
            message.error('Erro ao conectar com o servidor');
        }
    };

    const fetchHistorico = async () => {
        setLoadingHistorico(true);
        try {
            const response = await fetch(`${import.meta.env.VITE_API_TODO}/Order/busca/historicos-movimentacoes`);

            if (response.status == 200) {
                const data = await response.json();
                const formattedData = data.map((item: Movimentacao) => ({
                    ...item,
                    preco: new Intl.NumberFormat('pt-BR', {
                        style: 'currency',
                        currency: 'BRL'
                    }).format(item.preco),
                    data_criacao: new Date(item.data_criacao).toLocaleString('pt-BR'),
                    statusText: item.status === 0 ? 'REJEITADO' : 'PROCESSADO',
                }));
    
                setHistoricoMovimentacoes(formattedData);
                setIsModalVisible(true);
            }
        } catch (error) {
            message.error('Erro ao carregar histórico de movimentações');
        } finally {
            setLoadingHistorico(false);
        }
    };

    const historicoButton = (
        <Button
            type="text"
            icon={<HistoryOutlined />}
            onClick={fetchHistorico}
            loading={loadingHistorico}
        />
    );

    const deletarButton = (
        <Popconfirm
            title="Limpar tabela (Isso só acontece pois é um ambiente de aprendizado)"
            description="Tem certeza que deseja deletar todas as movimentações?"
            onConfirm={handleDelete}
            okText="Sim"
            cancelText="Não"
        >
            <Button
                type="text"
                danger
                icon={<DeleteOutlined />}
            />
        </Popconfirm>
    );

    return (<>
        <Card
            title="Movimentações por Ativos"
            style={{ marginTop: 16, maxWidth: 800, margin: '16px auto' }}
            extra={
                <div style={{ display: 'flex', gap: '8px' }}>
                    {historicoButton}
                    {deletarButton}
                </div>
            }
        >
            <Table
                dataSource={movimentacao}
                columns={columns}
                pagination={false}
                rowKey="ativo" />
        </Card>
        <Modal
            title="Histórico de Movimentações"
            open={isModalVisible}
            onCancel={() => setIsModalVisible(false)}
            width={1000}
            footer={null}
        >
            <Table
                dataSource={historicoMovimentacoes}
                columns={historicoColumns}
                pagination={{ pageSize: 5 }}
                rowKey={(record) => `${record.ativo}-${record.data_criacao}`}
                rowClassName={(record) =>
                    record.status === 0 ? 'table-row-error' : 'table-row-success'
                }
            />
        </Modal>
    </>
    );
};

export default MovimentacaoCard;
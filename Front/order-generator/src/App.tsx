import { Layout } from 'antd'
import OrderGenerator from './pages/OrderGenerator'
import { StockOutlined } from '@ant-design/icons';
import './index.css'

const { Header, Content } = Layout

export default function App() {
  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ color: 'white', fontSize: '1.5rem', background: '#2f3a2f' }}>
      <StockOutlined /> OrderGenerator
      </Header>
      <Content style={{ padding: '24px', background: '#f3f4eb' }}>
        <OrderGenerator />
      </Content>
    </Layout>
  )
}
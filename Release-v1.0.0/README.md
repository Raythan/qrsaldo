# 🎪 QRSaldo - Sistema de Saldo via QR Code para Quermesses

![QRSaldo](https://img.shields.io/badge/QRSaldo-v1.0-blue?style=flat-square&logo=qr-code)
![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=dotnet)
![SQLite](https://img.shields.io/badge/SQLite-Database-green?style=flat-square&logo=sqlite)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-blue?style=flat-square&logo=bootstrap)

Sistema completo de saldo via QR Code para quermesses, festivais e eventos. Elimine filas de troco, agilize vendas e ofereça mais praticidade aos visitantes!

## 🌟 Características Principais

- ✅ **100% Offline** - Funciona apenas com rede Wi-Fi local
- ✅ **Seguro** - Tokens com assinatura digital HMAC SHA256
- ✅ **Mobile First** - Interfaces otimizadas para celular
- ✅ **Tempo Real** - Atualizações instantâneas dos pedidos
- ✅ **Fácil Instalação** - Um único comando para rodar
- ✅ **API Completa** - Documentação Swagger incluída

## 🚀 Como Funciona

### 1. Cliente Paga no Caixa
- Cliente paga em dinheiro no caixa central
- Caixa gera QR Code com valor creditado
- Token possui validade configurável (padrão: 1 hora)

### 2. Cliente Escaneia QR Code
- Cliente escaneia QR Code com celular
- Saldo é creditado automaticamente
- Sistema cria conta automaticamente se necessário

### 3. Cliente Faz Compras
- Nas barracas, cliente escaneia QR Code do produto
- Escolhe quantidade e confirma compra
- Saldo é debitado e pedido é gerado

### 4. Barraca Recebe Pedido
- Barraca recebe pedido em tempo real
- Atualiza status: Pendente → Preparando → Pronto → Entregue
- Cliente é chamado pelo número do pedido

## 🏗️ Arquitetura do Sistema

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Interface     │    │   Interface     │    │   Interface     │
│    do Caixa     │    │   do Usuário    │    │   das Barracas  │
│                 │    │                 │    │                 │
│ • Gerar tokens  │    │ • Ver saldo     │    │ • Ver pedidos   │
│ • QR de crédito │    │ • Fazer compras │    │ • Atualizar     │
│ • Relatórios    │    │ • Histórico     │    │   status        │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
                    ┌─────────────────┐
                    │  ASP.NET Core   │
                    │   Web API       │
                    │                 │
                    │ • Controllers   │
                    │ • Services      │
                    │ • Token Mgmt    │
                    │ • QR Generator  │
                    └─────────────────┘
                                 │
                    ┌─────────────────┐
                    │ SQLite Database │
                    │                 │
                    │ • Usuários      │
                    │ • Barracas      │
                    │ • Produtos      │
                    │ • Pedidos       │
                    │ • Transações    │
                    │ • Tokens        │
                    └─────────────────┘
```

## 🛠️ Instalação e Execução

### Pré-requisitos
- .NET 8.0 SDK
- Qualquer sistema operacional (Windows, Linux, macOS)

### Passos para Instalação

1. **Clone o repositório**
   ```bash
   git clone https://github.com/seu-usuario/qrsaldo.git
   cd qrsaldo
   ```

2. **Execute a aplicação**
   ```bash
   cd QRSaldo.API
   dotnet run
   ```

3. **Acesse o sistema**
   - Aplicação: http://localhost:5041
   - API Docs: http://localhost:5041/swagger

### Configuração da Rede

Para usar em uma quermesse real:

1. **Configure um roteador Wi-Fi local** (sem internet)
2. **Execute a aplicação em um notebook/PC**
3. **Conecte todos os dispositivos na mesma rede**
4. **Acesse via IP local** (ex: http://192.168.1.100:5041)

## 🎯 Interfaces do Sistema

### 🏪 Interface do Caixa (`/caixa`)
- Gerar tokens de crédito após receber pagamento
- Definir validade dos tokens (1-1440 minutos)
- QR Codes para recarga de saldo
- Histórico de tokens gerados

### 👤 Interface do Usuário (`/usuario`)
- Login por telefone (cria conta automaticamente)
- Visualizar saldo atual e histórico
- Navegar pelo cardápio das barracas
- Fazer compras escaneando QR Codes
- Acompanhar status dos pedidos

### 🍔 Interface das Barracas (`/barraca/{id}`)
- Lista de pedidos em tempo real
- Filtros por status (Pendente, Preparando, Pronto, etc.)
- Atualizar status dos pedidos
- Auto-refresh a cada 30 segundos
- Informações completas do cliente

## 📱 QR Codes do Sistema

### QR Code de Crédito
```
http://localhost:5041/creditar?token=eyJ0aW1lc3RhbXA...
```
- Gerado pelo caixa após receber pagamento
- Possui validade configurável
- Token assinado digitalmente

### QR Code de Produto
```
http://localhost:5041/consumir?produtoId=1
```
- Um QR Code para cada produto
- Cliente escaneia para fazer compra
- Pode ser impresso e colado na barraca

## 🗃️ Estrutura do Banco de Dados

### Tabelas Principais

| Tabela | Descrição |
|--------|-----------|
| `Usuarios` | Dados dos clientes (telefone, nome, saldo) |
| `Barracas` | Informações das barracas do evento |
| `Produtos` | Cardápio com preços de cada barraca |
| `Pedidos` | Pedidos realizados com status |
| `Transacoes` | Histórico de créditos e débitos |
| `TokensCredito` | Tokens de crédito com validade |

### Relacionamentos
- Usuario 1:N Pedidos
- Usuario 1:N Transacoes  
- Barraca 1:N Produtos
- Barraca 1:N Pedidos
- Produto 1:N Pedidos

## 🔐 Segurança

### Tokens de Crédito
- **Assinatura HMAC SHA256** - Previne falsificação
- **Validade configurável** - Tokens expiram automaticamente
- **Uso único** - Cada token só pode ser usado uma vez
- **Auditoria completa** - Histórico de quem usou quando

### Validações
- Saldo insuficiente é rejeitado
- Produtos/barracas inativas são bloqueados
- Tokens expirados são rejeitados
- Duplicação de pedidos é evitada

## 🎨 Customização

### Adicionar Novas Barracas
```sql
INSERT INTO Barracas (Nome, Descricao, Ativa) 
VALUES ('Minha Barraca', 'Descrição da barraca', 1);
```

### Adicionar Produtos
```sql
INSERT INTO Produtos (Nome, Preco, BarracaId, Ativo) 
VALUES ('Meu Produto', 15.50, 1, 1);
```

### Personalizar Interface
- Edite os arquivos em `wwwroot/`
- CSS/JS customizados são suportados
- Cores e layout podem ser alterados

## 📊 Relatórios e Controle

### Controle Financeiro
- **Saldo Total Gerado**: Soma de todos os tokens criados
- **Saldo em Circulação**: Saldo atual nos telefones dos clientes  
- **Saldo Consumido**: Total de compras realizadas
- **Quebra de Caixa**: Diferença entre gerado e consumido

### Relatórios por Barraca
- Vendas por produto
- Pedidos por status
- Faturamento por período
- Produtos mais vendidos

## 🛟 Solução de Problemas

### Aplicação não inicia
```bash
# Verificar versão do .NET
dotnet --version

# Limpar e recompilar
dotnet clean
dotnet build
dotnet run
```

### Banco de dados com erro
```bash
# Deletar banco e recriar
rm qrsaldo.db
dotnet run
```

### QR Code não funciona
- Verificar se dispositivos estão na mesma rede
- Confirmar IP correto na URL
- Testar conectividade entre dispositivos

## 🎯 Funcionalidades Futuras

- [ ] Scanner de QR Code nativo (câmera)
- [ ] Notificações push para barracas
- [ ] Relatórios em PDF/Excel
- [ ] Backup automático
- [ ] Múltiplos pontos de caixa
- [ ] Sistema de filas por barraca
- [ ] Integração com impressoras térmicas

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

## 📞 Suporte

Para dúvidas ou suporte:
- Abra uma [Issue](https://github.com/seu-usuario/qrsaldo/issues)
- Entre em contato: seu-email@exemplo.com

---

**QRSaldo** - Transformando a experiência de compras em eventos! 🎪✨

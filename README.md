# Dev.com

Plataforma moderna que conecta empresas a desenvolvedores freelancers qualificados. Empresas publicam projetos com transparência de orçamento e prazo, desenvolvedores enviam propostas competitivas e acompanham tudo em tempo real através de dashboards intuitivos com notificações instantâneas.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Stack Tecnológico](#stack-tecnológico)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Contas de Demonstração](#contas-de-demonstração)
- [Endpoints principais](#endpoints-principais)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Padrões e Boas Práticas](#padrões-e-boas-práticas)
- [Segurança](#segurança)
- [Deploy](#deploy)
- [Troubleshooting](#troubleshooting)
- [Contribuindo](#contribuindo)

## 🎯 Visão Geral

**Dev.com** é uma plataforma SaaS (Software as a Service) que facilita a conexão entre empresas que buscam desenvolvedores e profissionais freelancers que querem trabalhos. O sistema oferece:

- **Para Empresas:** Publicação simplificada de projetos, recebimento de propostas qualificadas e gerenciamento de contratações
- **Para Desenvolvedores:** Descoberta de oportunidades, envio de propostas competitivas e acompanhamento de projetos
- **Para Ambos:** Notificações em tempo real, perfis públicos, histórico de transações e sistema de feedback

## 🛠️ Stack Tecnológico

### Frontend
- **HTML5, CSS3 e JavaScript vanilla** — páginas estáticas e de autenticação
- **React 18 + Vite** — app interativa de listagem e gerenciamento de projetos
- **Serve** — servidor de desenvolvimento leve e eficiente
- **Fetch API** — consumo de endpoints REST
- **LocalStorage** — persistência de token JWT no cliente
- **Responsive Design** — suporte a desktop, tablet e mobile

### Backend
- **.NET 8** — framework robusto e de alta performance
- **Clean Architecture** — separação em camadas Domain → Application → Infrastructure → API
- **PostgreSQL 18** — banco de dados relacional com suporte a JSON e fulltext search
- **Entity Framework Core 8** — ORM type-safe com migrations automáticas
- **Npgsql** — provider PostgreSQL nativo para .NET
- **Redis** — cache em memória para listagens, perfis e sessões
- **JWT (HMAC-SHA256)** — autenticação stateless e segura
- **BCrypt (work-factor 12)** — hash de senhas com salting
- **CORS** — controle de acesso entre origens

### Banco de Dados
- **PostgreSQL 18** na porta `5432`
- **Migrations automáticas** via Entity Framework
- **Índices otimizados** em campos de busca frequente
- **Constraints de integridade referencial**

### Cache
- **Redis** na porta `6379`
- **TTL configurável** para diferentes tipos de dados
- **Invalidação automática** ao criar/atualizar recursos

## 📦 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **[Node.js](https://nodejs.org/)** versão 18+ (com npm 9+)
- **[.NET SDK](https://dotnet.microsoft.com/)** versão 8.0 ou superior
- **[PostgreSQL](https://www.postgresql.org/)** versão 18 rodando na porta `5432`
- **[Redis](https://redis.io/)** versão 7+ rodando na porta `6379`
- **[Git](https://git-scm.com/)** para versionamento

### Verificar instalações

```bash
# Node.js
node --version
npm --version

# .NET
dotnet --version

# PostgreSQL
psql --version

# Redis
redis-cli --version
```

## ⚙️ Instalação e Execução

### Passo 1: Clonar o Repositório

```bash
git clone https://github.com/seu-usuario/Dev.com.git
cd Dev.com
```

### Passo 2: Configurar o Banco de Dados

Crie o banco de dados e as credenciais:

```sql
CREATE DATABASE "Dev.com";
CREATE USER postgres WITH PASSWORD '240505';
ALTER ROLE postgres SUPERUSER CREATEDB CREATEROLE;
```

Ou se preferir usando `psql`:

```bash
psql -U postgres -c "CREATE DATABASE \"Dev.com\";"
```

### Passo 3: Configurar Backend

```bash
cd Dev.com/Backend

# Restaurar dependências
dotnet restore

# Aplicar migrations (automático na inicialização)
dotnet run --project DevCom.API
```

A API estará disponível em: `http://localhost:5000`

**Verificar saúde da API:**
```bash
curl http://localhost:5000/health
```

### Passo 4: Configurar Frontend

```bash
cd ../Frontend

# Instalar dependências
npm install

# Iniciar servidor de desenvolvimento
npm start
```

O frontend estará disponível em: `http://localhost:5500`

### Passo 5: Verificar Conexões

```bash
# Redis
redis-cli ping
# Esperado: PONG

# PostgreSQL
psql -U postgres -d "Dev.com" -c "SELECT version();"

# API Health Check
curl http://localhost:5000/health
```

**Observações importantes:**
- O banco `Dev.com` é criado automaticamente pelas migrations do Entity Framework
- Redis deve estar rodando em background
- PostgreSQL utiliza usuário `postgres` com senha `240505`
- Todas as tabelas e índices são criados na primeira execução

## 👥 Contas de Demonstração

Para testar a plataforma, use as seguintes contas pré-configuradas:

| Papel | E-mail | Senha | Perfil |
|---|---|---|---|
| 🏢 Empresa (Client) | ana@empresa.com | Test@1234 | Acesso completo a criação e gerenciamento de projetos |
| 👨‍💻 Desenvolvedor (Dev) | pedro@dev.com | Test@1234 | Acesso completo a busca de projetos e envio de propostas |

**Fluxo de teste recomendado:**
1. Login como empresa (ana@empresa.com)
2. Criar um novo projeto
3. Logout e login como desenvolvedor (pedro@dev.com)
4. Buscar o projeto criado e enviar uma proposta
5. Voltar ao login da empresa e aceitar a proposta
6. Verificar notificações em ambos os lados

## 🔌 Endpoints principais

### Autenticação

| Método | Endpoint | Auth | Descrição | Status |
|---|---|---|---|---|
| POST | `/api/auth/register` | — | Cadastro novo (role: Client ou Dev) | ✅ |
| POST | `/api/auth/login` | — | Login com e-mail/senha — retorna JWT | ✅ |
| POST | `/api/auth/reset-password` | — | Redefinir senha com token de recuperação | ✅ |
| POST | `/api/auth/verify-email` | — | Verificar e-mail via token | ✅ |

### Projetos

| Método | Endpoint | Auth | Descrição | Status |
|---|---|---|---|---|
| GET | `/api/projects` | — | Listagem paginada com filtros (categoria, status, budget) | ✅ |
| GET | `/api/projects/:id` | — | Detalhe completo do projeto com propostas | ✅ |
| POST | `/api/projects` | Client ✓ | Publicar novo projeto | ✅ |
| GET | `/api/projects/my` | Client ✓ | Listar meus projetos (filtros e paginação) | ✅ |
| PATCH | `/api/projects/:id` | Client ✓ | Atualizar projeto (apenas publicador) | ✅ |
| DELETE | `/api/projects/:id` | Client ✓ | Cancelar projeto (apenas publicador) | ✅ |

### Propostas

| Método | Endpoint | Auth | Descrição | Status |
|---|---|---|---|---|
| GET | `/api/projects/:id/proposals` | Client ✓ | Listar propostas de um projeto | ✅ |
| POST | `/api/projects/:id/proposals` | Dev ✓ | Enviar nova proposta | ✅ |
| GET | `/api/proposals/my` | Dev ✓ | Minhas propostas (com filtros) | ✅ |
| PATCH | `/api/proposals/:id/accept` | Client ✓ | Aceitar proposta e encerrar outras | ✅ |
| PATCH | `/api/proposals/:id/reject` | Client ✓ | Rejeitar proposta | ✅ |
| DELETE | `/api/proposals/:id` | Dev ✓ | Cancelar proposta enviada | ✅ |

### Notificações

| Método | Endpoint | Auth | Descrição | Status |
|---|---|---|---|---|
| GET | `/api/notifications` | Auth ✓ | Listar notificações do usuário (paginadas) | ✅ |
| GET | `/api/notifications/unread-count` | Auth ✓ | Contador de notificações não lidas | ✅ |
| PATCH | `/api/notifications/:id/read` | Auth ✓ | Marcar notificação como lida | ✅ |
| PATCH | `/api/notifications/read-all` | Auth ✓ | Marcar todas como lidas | ✅ |

### Perfis de Desenvolvedores

| Método | Endpoint | Auth | Descrição | Status |
|---|---|---|---|---|
| GET | `/api/devs/:email` | — | Perfil público do desenvolvedor com skills e histórico | ✅ |
| GET | `/api/devs/:email/projects` | — | Projetos completados por um desenvolvedor | ✅ |
| GET | `/api/devs/:email/reviews` | — | Feedbacks deixados para um desenvolvedor | ✅ |

### Saúde e Monitoramento

| Método | Endpoint | Auth | Descrição | Status |
|---|---|---|---|---|
| GET | `/health` | — | Health check geral (API + DB + Redis) | ✅ |
| GET | `/api/health/db` | — | Verificar conexão com PostgreSQL | ✅ |
| GET | `/api/health/redis` | — | Verificar conexão com Redis | ✅ |

**Exemplo de requisição com autenticação:**

```bash
curl -X GET http://localhost:5000/api/projects/my \
  -H "Authorization: Bearer SEU_JWT_TOKEN_AQUI"
```

**Exemplo de criação de projeto:**

```bash
curl -X POST http://localhost:5000/api/projects \
  -H "Authorization: Bearer SEU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "App React para E-commerce",
    "description": "Preciso de um app React com integração de pagamento",
    "budget": 5000,
    "deadline": "2024-12-31",
    "category": "Web Development",
    "tags": ["React", "JavaScript", "E-commerce"]
  }'
```

## 📁 Estrutura do Projeto

```
Dev.com/
├── Backend/
│   ├── DevCom.API/
│   │   ├── Controllers/                 # Endpoints da API
│   │   │   ├── AuthController.cs        # Autenticação e registro
│   │   │   ├── ProjectsController.cs    # CRUD de projetos
│   │   │   ├── ProposalsController.cs   # CRUD de propostas
│   │   │   ├── NotificationsController.cs # Notificações
│   │   │   └── DevsController.cs        # Perfis públicos
│   │   ├── Middleware/
│   │   │   └── ExceptionMiddleware.cs   # Tratamento global de erros
│   │   ├── Program.cs                   # Configuração da aplicação
│   │   ├── appsettings.json             # Configurações gerais
│   │   ├── appsettings.Development.json # Configurações de dev
│   │   └── DevCom.API.csproj            # Referências do projeto
│   │
│   ├── DevCom.Domain/                   # Camada de domínio
│   │   ├── Entities/                    # Entidades do negócio
│   │   │   ├── User.cs
│   │   │   ├── Project.cs
│   │   │   ├── Proposal.cs
│   │   │   └── Notification.cs
│   │   ├── Enums/                       # Enumerações
│   │   │   ├── UserRole.cs
│   │   │   ├── ProjectStatus.cs
│   │   │   └── ProposalStatus.cs
│   │   └── ValueObjects/                # Objetos de valor
│   │
│   ├── DevCom.Application/              # Camada de aplicação
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   ├── Projects/
│   │   │   ├── Proposals/
│   │   │   └── Notifications/
│   │   ├── Services/                    # Lógica de negócio
│   │   │   ├── AuthService.cs
│   │   │   ├── ProjectService.cs
│   │   │   ├── ProposalService.cs
│   │   │   └── NotificationService.cs
│   │   ├── Interfaces/                  # Contratos
│   │   └── Validators/                  # Validação de entrada
│   │
│   ├── DevCom.Infrastructure/           # Camada de infraestrutura
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs          # Contexto do EF Core
│   │   │   └── Migrations/              # Histórico de migrations
│   │   ├── Repositories/                # Padrão Repository
│   │   ├── Services/
│   │   │   ├── TokenService.cs          # JWT
│   │   │   ├── PasswordService.cs       # BCrypt
│   │   │   └── CacheService.cs          # Redis
│   │   └── Extensions/                  # Extensões e helpers
│   │
│   └── DevCom.sln                       # Solução Visual Studio
│
├── Frontend/
│   ├── index.html                       # Página inicial / landing
│   ├── auth.html                        # Login, cadastro, recuperação
│   ├── dashboard.html                   # Dashboard universal (Client/Dev)
│   ├── projeto-detalhes.html            # Visualização de projeto
│   ├── projetos.html                    # Listagem de projetos
│   ├── dev-perfil.html                  # Perfil público do dev
│   ├── js/
│   │   ├── main.js                      # Cliente HTTP e helpers globais
│   │   ├── auth.js                      # Lógica de autenticação
│   │   ├── api.js                       # Funções de API
│   │   └── utils.js                     # Utilitários diversos
│   ├── css/
│   │   ├── styles.css                   # Estilos globais
│   │   └── responsive.css               # Media queries
│   ├── projetos-app/                    # App React (source)
│   │   ├── src/
│   │   │   ├── components/
│   │   │   ├── pages/
│   │   │   ├── hooks/
│   │   │   ├── App.jsx
│   │   │   └── main.jsx
│   │   ├── package.json
│   │   └── vite.config.js
│   ├── projetos-app-build/              # Build otimizado do React
│   ├── netlify.toml                     # Configuração Netlify
│   └── package.json
│
├── .gitignore                           # Arquivos ignorados pelo Git
├── .editorconfig                        # Configuração de editor
├── README.md                            # Este arquivo
└── LICENSE                              # Licença do projeto
```

### Descrição das Camadas (.NET)

#### **Domain** (DevCom.Domain)
Contém as entidades, enums e value objects. Sem dependências externas. Representa as regras de negócio puras.

#### **Application** (DevCom.Application)
Implementa os casos de uso e serviços. DTOs para transferência de dados. Valida dados de entrada e coordena chamadas para infraestrutura.

#### **Infrastructure** (DevCom.Infrastructure)
Implementa EF Core, repositórios, JWT, BCrypt, Redis. Acesso a banco de dados, APIs externas e serviços de terceiros.

#### **API** (DevCom.API)
Controllers, middleware, configuração da aplicação. Ponto de entrada REST. Injeta dependências e orquestra as camadas.

## 🎨 Arquitetura

### Fluxo de Autenticação

```
1. Usuário faz login
   ↓
2. Backend valida credenciais vs BCrypt hash
   ↓
3. Backend gera JWT (HMAC-SHA256) com userId e role
   ↓
4. Frontend armazena JWT em localStorage
   ↓
5. Requisições posteriores enviam Authorization: Bearer JWT
   ↓
6. Backend valida assinatura JWT a cada requisição
```

### Fluxo de Projeto

```
Cliente publica projeto
   ↓
Projeto aparece em listagem (Redis cache)
   ↓
Dev vê projeto e envia proposta
   ↓
Cliente recebe notificação (em tempo real)
   ↓
Cliente aceita proposta
   ↓
Projeto muda para "Em andamento"
   ↓
Dev recebe notificação de aceite
   ↓
Ambos podem marcar como concluído
```

### Cache Strategy

- **Listagem de projetos:** TTL 15 minutos
- **Perfil de dev:** TTL 1 hora
- **Notificações:** TTL 5 minutos
- **Sessão de usuário:** TTL 24 horas

## ⭐ Funcionalidades

### 🏢 Como Empresa (Client)

- ✅ Publicar projetos com titulo, descrição, orçamento, prazo e categorias
- ✅ Adicionar tags para melhor descoberta pelos devs
- ✅ Editar e cancelar projetos não iniciados
- ✅ Receber notificações em tempo real de novas propostas
- ✅ Visualizar todas as propostas recebidas com perfil do dev
- ✅ Aceitar proposta — projeto muda automaticamente para "Em andamento"
- ✅ Rejeitar ou arquivar propostas não selecionadas
- ✅ Dashboard com status de todos os seus projetos
- ✅ Histórico completo de projetos e gastos
- ✅ Perfil público com histórico de contratações

### 👨‍💻 Como Desenvolvedor (Dev)

- ✅ Navegar e filtrar projetos abertos por categoria, orçamento, prazo
- ✅ Busca full-text em títulos e descrições de projetos
- ✅ Paginação eficiente com carregamento progressivo
- ✅ Enviar propostas com valor, prazo estimado e mensagem personalizada
- ✅ Acompanhar status de cada proposta no dashboard
- ✅ Receber notificação quando proposta for aceita
- ✅ Manter perfil público com skills, experiência e feedback
- ✅ Visualizar histórico de projetos completados
- ✅ Ver feedbacks deixados por clientes
- ✅ Gerenciar múltiplas propostas simultâneas

### 👥 Para Ambos

- ✅ Autenticação segura com JWT
- ✅ Recuperação de senha por e-mail
- ✅ Verificação de e-mail
- ✅ Dashboard responsivo (desktop, tablet, mobile)
- ✅ Sistema de notificações em tempo real
- ✅ Perfis públicos e histórico transparente
- ✅ Interface intuitiva e acessível
- ✅ Suporte multi-idioma (base pronta)

## 🔐 Padrões e Boas Práticas

### Código

- **Clean Architecture:** Separação clara de responsabilidades em camadas
- **Repository Pattern:** Abstração da camada de dados
- **Dependency Injection:** Inversão de controle via container DI do .NET
- **SOLID Principles:**
  - Single Responsibility
  - Open/Closed
  - Liskov Substitution
  - Interface Segregation
  - Dependency Inversion
- **DTOs:** Transfer objects para não expor entities
- **Validators:** Fluent Validation para entrada de dados
- **Async/Await:** Operações assíncronas em I/O

### Banco de Dados

- **Migrations:** Versionamento automático de schema
- **Índices:** Otimizados em campos de busca
- **Foreign Keys:** Integridade referencial
- **Soft Delete:** Remoção lógica de dados críticos
- **Timestamps:** created_at e updated_at em todas as tabelas

### Frontend

- **SPA Pattern:** Single Page Application com React
- **Component-Based:** Componentes reutilizáveis
- **State Management:** LocalStorage para persistência
- **Error Handling:** Try-catch e validação de resposta
- **Responsive Design:** Mobile-first approach

## 🔒 Segurança

### Autenticação & Autorização

- **JWT Stateless:** Não requer sessão no servidor
- **HMAC-SHA256:** Assinatura criptográfica de tokens
- **BCrypt:** Hashing de senhas com salt aleatório (work-factor 12)
- **Token Expiry:** Tokens expiram após tempo configurável
- **Role-Based Access Control (RBAC):** Client vs Dev

### Validação & Sanitização

- **Input Validation:** Fluent Validators em todos os endpoints
- **SQL Injection Prevention:** Entity Framework + Parameterized Queries
- **XSS Protection:** Sanitização de entrada no frontend
- **CORS:** Apenas origens autorizadas

### Comunicação

- **HTTPS:** Recomendado em produção (Netlify force HTTPS)
- **Sensitive Data:** Não enviado em logs ou URLs
- **Password Reset:** Token único com expiração

## 🚀 Deploy

### Frontend (Netlify)

O frontend está otimizado para deploy automático:

```bash
# Build local
cd Frontend/projetos-app
npm run build

# Deploy via CLI
netlify deploy --prod --dir=dist
```

**Link em produção:** https://dev-com00.netlify.app/

**Configuração em `netlify.toml`:**
- Build command: `npm run build`
- Publish directory: `dist`
- Redirecionamentos automáticos para SPA

### Backend (Azure / AWS / DigitalOcean)

```bash
# Build para produção
dotnet publish -c Release -o ./publish

# Docker (opcional)
docker build -t devcom-api:latest .
docker run -p 5000:5000 devcom-api:latest
```

**Variáveis de ambiente necessárias:**
```env
ConnectionStrings__DefaultConnection=Server=prod-db;Database=Dev.com;...
JwtSecret=sua-chave-secreta-aqui
RedisConnection=prod-redis:6379
```

## 🐛 Troubleshooting

### Backend não conecta ao PostgreSQL

```bash
# Verificar se PostgreSQL está rodando
psql -U postgres -c "SELECT version();"

# Verificar credenciais em appsettings.Development.json
# Padrão: Server=localhost;Port=5432;Database=Dev.com;User Id=postgres;Password=240505;

# Criar banco manualmente se necessário
psql -U postgres -c "CREATE DATABASE \"Dev.com\";"
```

### Redis connection refused

```bash
# Verificar se Redis está rodando
redis-cli ping

# Iniciar Redis (Windows)
redis-server

# Ou via WSL
wsl redis-server
```

### Frontend não vê API

```bash
# Verificar se backend está respondendo
curl http://localhost:5000/health

# Verificar URL da API em Frontend/js/main.js
# Deve apontar para http://localhost:5000

# Limpar cache do navegador (Ctrl+Shift+Delete)
```

### Erro 401 Unauthorized

```bash
# Token expirou — fazer logout e login novamente
# LocalStorage > devcom_token está vazio?
# Verificar se está sendo enviado: Authorization: Bearer TOKEN
```

### Migrations travadas

```bash
# Remover migrations problemáticas
dotnet ef migrations remove --project DevCom.API

# Resetar banco (CUIDADO - deleta tudo)
dotnet ef database drop --project DevCom.API
dotnet ef database update --project DevCom.API
```

### Padrão de Commits

```
feat: Adiciona nova funcionalidade
fix: Corrige bug em X
docs: Atualiza documentação
style: Reformata código sem mudar lógica
refactor: Refatora componente X
test: Adiciona testes para Y
chore: Atualiza dependências
```

## 📄 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo LICENSE para detalhes.

## 👤 Autor

**Lucas Frotté Lafin**
- Email: lucasfrotte9*@gmail.com
- Linkedin: www.linkedin.com/in/lucas-frotté-lafin

---

**Última atualização:** Setembro 2026 | **Versão:** 1.0.0

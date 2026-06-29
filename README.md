# Dev.com

# Alunos
- Lucas Frotte Lafin - 06010493
- Ana Luiza Maciel Mattos - 06009322
- Pedro Nogueira Teodosio - 06010196
- Alexandre dos Santos - 06010479

Plataforma que conecta empresas a desenvolvedores freelancers. Empresas publicam projetos, devs enviam propostas e acompanham o andamento pelo dashboard.

## Tecnologias

- HTML, CSS e JavaScript vanilla (páginas principais)
- React + Vite (listagem e detalhes de projetos)
- React Router DOM (navegação SPA)
- localStorage (persistência de dados)
- serve (servidor de desenvolvimento)

## Pré-requisitos

- [Node.js](https://nodejs.org/) versão 18 ou superior
- npm

## Instalação

Clone o repositório e instale as dependências da raiz:

```bash
git clone https://github.com/LucasFrotteLafin/Dev.com.git
cd Dev.com
npm install
```

Instale também as dependências do app React:

```bash
cd projetos-app
npm install
cd ..
```

## Rodando o projeto

Na raiz do projeto, execute:

```bash
npm start
```

O servidor sobe em `http://localhost:3000`. Se a porta 3000 estiver ocupada, o serve escolhe outra automaticamente e exibe a URL no terminal.

## Build do app React

O diretório `projetos-app-build/` já contém o build mais recente. Se você fizer alterações no código React (`projetos-app/src/`), rode o build novamente:

```bash
npm run build
```

Ou diretamente dentro do `projetos-app/`:

```bash
cd projetos-app
npm run build
```

O build é gerado automaticamente em `projetos-app-build/`.

## Estrutura do projeto

```
Dev.com/
├── index.html              # Página inicial
├── auth.html               # Login e cadastro
├── dashboard.html          # Dashboard do usuário (dev ou cliente)
├── projeto-detalhes.html   # Detalhes de um projeto (vanilla)
├── projetos.html           # Listagem de projetos (vanilla, legado)
├── dev-perfil.html         # Perfil público do desenvolvedor
├── css/
│   └── style.css           # Estilos globais
├── js/
│   ├── auth.js             # Autenticação (localStorage)
│   ├── main.js             # Utilitários e dados mock
│   └── store.js            # Store central (vanilla)
├── projetos-app/           # App React (código fonte)
│   └── src/
│       ├── pages/
│       │   ├── Projetos.jsx        # Listagem de projetos
│       │   └── ProjetoDetalhes.jsx # Detalhes do projeto
│       ├── components/
│       │   ├── Nav.jsx             # Navbar
│       │   └── ProposalModal.jsx   # Modal de proposta
│       └── store.js        # Store React (localStorage)
├── projetos-app-build/     # Build do app React (gerado)
├── serve.json              # Configuração do servidor serve
└── package.json
```

## Funcionalidades

**Como empresa (cliente)**
- Criar conta com papel de "Empresa"
- Publicar projetos com título, descrição, orçamento, prazo e tecnologias
- Visualizar propostas recebidas no dashboard
- Aceitar uma proposta (projeto passa para "Em andamento")
- Receber notificações de novas propostas

**Como desenvolvedor**
- Criar conta com papel de "Desenvolvedor"
- Navegar pelos projetos disponíveis com filtro por categoria e busca
- Enviar proposta com valor, prazo e mensagem
- Acompanhar status das propostas enviadas no dashboard
- Receber notificação quando uma proposta for aceita
- Ter perfil público acessível pelos clientes

## Contas de demonstração

O projeto usa localStorage — não há backend. Para testar, crie uma conta pela página de login (`/auth.html`) selecionando o papel desejado (Empresa ou Desenvolvedor).

Os projetos mock são carregados automaticamente no primeiro acesso.

## Deploy

O projeto está configurado para deploy no [Netlify](https://www.netlify.com/) via `netlify.toml`

Link do site : https://dev-com00.netlify.app/

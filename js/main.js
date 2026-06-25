const mockProjects = [
  {
    id: 1,
    title: "E-commerce para loja de roupas",
    client: "Ana Silva",
    description: "Preciso de um e-commerce completo com carrinho, pagamento e painel administrativo.",
    budget: "R$ 3.500",
    deadline: "30 dias",
    category: "E-commerce",
    status: "open",
    tags: ["React", "Node.js", "MongoDB"],
    proposals: 4,
    createdAt: "2 dias atrás",
  },
  {
    id: 2,
    title: "App de agendamento para barbearia",
    client: "Carlos Mendes",
    description: "Sistema de agendamento online onde clientes possam marcar horários e receber confirmação.",
    budget: "R$ 1.800",
    deadline: "20 dias",
    category: "Web App",
    status: "open",
    tags: ["JavaScript", "HTML", "CSS"],
    proposals: 7,
    createdAt: "1 dia atrás",
  },
  {
    id: 3,
    title: "Dashboard de analytics",
    client: "Empresa TechData",
    description: "Dashboard para visualização de dados de vendas com gráficos e exportação em PDF.",
    budget: "R$ 2.200",
    deadline: "25 dias",
    category: "Dashboard",
    status: "in_progress",
    tags: ["React", "Chart.js"],
    proposals: 2,
    createdAt: "5 dias atrás",
  },
  {
    id: 4,
    title: "Landing page para startup",
    client: "StartupX",
    description: "Landing page moderna e responsiva com formulário de captação de leads.",
    budget: "R$ 800",
    deadline: "10 dias",
    category: "Landing Page",
    status: "open",
    tags: ["HTML", "CSS", "JavaScript"],
    proposals: 11,
    createdAt: "3 horas atrás",
  },
  {
    id: 5,
    title: "Sistema de gestão escolar",
    client: "Colégio Futuro",
    description: "Sistema para gerenciar alunos, notas, frequência e comunicação com responsáveis.",
    budget: "R$ 5.000",
    deadline: "60 dias",
    category: "Sistema",
    status: "open",
    tags: ["React", "Python", "PostgreSQL"],
    proposals: 3,
    createdAt: "1 semana atrás",
  },
  {
    id: 6,
    title: "Integração com API de pagamento",
    client: "LojaRápida",
    description: "Integrar Stripe e Mercado Pago em plataforma existente com webhook automático.",
    budget: "R$ 1.200",
    deadline: "15 dias",
    category: "Integração",
    status: "completed",
    tags: ["Node.js", "Stripe"],
    proposals: 6,
    createdAt: "2 semanas atrás",
  },
  {
    id: 7,
    title: "Plataforma de cursos online",
    client: "EduTech Brasil",
    description: "Plataforma EAD com vídeo-aulas, quizzes, certificados e painel do aluno.",
    budget: "R$ 8.000",
    deadline: "45 dias",
    category: "Web App",
    status: "open",
    tags: ["React", "Node.js", "PostgreSQL"],
    proposals: 5,
    createdAt: "4 horas atrás",
  },
  {
    id: 8,
    title: "App de delivery para restaurante",
    client: "Sabor & Arte",
    description: "Aplicativo de pedidos online com rastreamento em tempo real e integração com iFood.",
    budget: "R$ 4.500",
    deadline: "35 dias",
    category: "Mobile",
    status: "open",
    tags: ["React Native", "Node.js", "Firebase"],
    proposals: 9,
    createdAt: "1 dia atrás",
  },
  {
    id: 9,
    title: "Sistema de RH e folha de pagamento",
    client: "Grupo Nexus",
    description: "Sistema completo para gestão de funcionários, férias, ponto eletrônico e folha de pagamento.",
    budget: "R$ 12.000",
    deadline: "90 dias",
    category: "Sistema",
    status: "open",
    tags: ["Vue.js", "Python", "PostgreSQL"],
    proposals: 2,
    createdAt: "2 dias atrás",
  },
  {
    id: 10,
    title: "Redesign de site institucional",
    client: "Escritório Jurídico Alves",
    description: "Modernização do site com novo layout, SEO otimizado e formulário de contato.",
    budget: "R$ 1.500",
    deadline: "15 dias",
    category: "Landing Page",
    status: "open",
    tags: ["HTML", "CSS", "JavaScript"],
    proposals: 6,
    createdAt: "5 horas atrás",
  },
  {
    id: 11,
    title: "API REST para fintech",
    client: "PayFlex",
    description: "Desenvolvimento de API para transferências, extrato e autenticação com dois fatores.",
    budget: "R$ 6.000",
    deadline: "40 dias",
    category: "Integração",
    status: "open",
    tags: ["Node.js", "TypeScript", "PostgreSQL"],
    proposals: 4,
    createdAt: "3 dias atrás",
  },
  {
    id: 12,
    title: "Dashboard de monitoramento IoT",
    client: "SmartFactory",
    description: "Painel para monitoramento de sensores industriais em tempo real com alertas automáticos.",
    budget: "R$ 7.500",
    deadline: "50 dias",
    category: "Dashboard",
    status: "open",
    tags: ["React", "WebSocket", "Node.js"],
    proposals: 1,
    createdAt: "6 horas atrás",
  },
];

function getStatusLabel(status) {
  if (status === "open") return "Aberto";
  if (status === "in_progress") return "Em andamento";
  return "Concluído";
}

function getStatusBadge(status) {
  if (status === "open") return "badge-green";
  if (status === "in_progress") return "badge-yellow";
  return "badge-purple";
}

function goTo(page, id) {
  if (id) {
    window.location.href = page + "?id=" + id;
  } else {
    window.location.href = page;
  }
}

function renderNav() {
  const session = typeof Auth !== "undefined" && Auth.getSession ? Auth.getSession() : null;
  const btn = document.querySelector("nav .nav-btn");
  if (!btn) return;
  if (session) {
    btn.textContent = session.name.split(" ")[0];
    btn.onclick = () => { window.location.href = "dashboard.html"; };
  } else {
    btn.textContent = "Entrar";
    btn.onclick = () => { window.location.href = "auth.html"; };
  }
}

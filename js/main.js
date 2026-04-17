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

// ── Store central ──────────────────────────────────────────────
const Store = (() => {
  const KEYS = {
    projects: "dvc_projects",
    notifications: "dvc_notifications",
    proposals: "dvc_proposals",
    devProfiles: "dvc_dev_profiles",
  };

  const DEMO_PROFILES = [
    {
      email: "joao@dev.com",
      name: "João Souza",
      bio: "Desenvolvedor full stack com 5 anos de experiência em aplicações web e mobile.",
      skills: ["React", "Node.js", "PostgreSQL", "REST API"],
      rating: 4.9,
      completedCount: 18,
      memberSince: "2022",
      feedbacks: [
        { company: "LojaRápida", stars: 5, text: "Entregou no prazo, código limpo. Recomendo muito!" },
        { company: "StartupX", stars: 5, text: "Ótima comunicação e resultado excelente." },
      ],
      pastProjects: [
        { title: "Integração com API de pagamento", tags: ["Node.js", "Stripe"], duration: "15 dias" },
        { title: "Dashboard de analytics", tags: ["React", "Chart.js"], duration: "25 dias" },
      ],
    },
    {
      email: "maria@dev.com",
      name: "Maria Lima",
      bio: "Especialista em front-end moderno e UX. Apaixonada por interfaces bonitas e acessíveis.",
      skills: ["Vue.js", "React", "TypeScript", "CSS"],
      rating: 4.7,
      completedCount: 12,
      memberSince: "2023",
      feedbacks: [
        { company: "Empresa TechData", stars: 5, text: "Interface ficou incrível, superou as expectativas." },
        { company: "Colégio Futuro", stars: 4, text: "Bom trabalho, comunicação poderia ser melhor." },
      ],
      pastProjects: [
        { title: "Landing page para startup", tags: ["HTML", "CSS", "JavaScript"], duration: "10 dias" },
        { title: "App de agendamento", tags: ["Vue.js", "Firebase"], duration: "22 dias" },
      ],
    },
  ];

  function seed() {
    // Merge mock projects — add any missing by id
    const existing = getProjects();
    const existingIds = new Set(existing.map(p => String(p.id)));
    const toAdd = mockProjects.filter(p => !existingIds.has(String(p.id)));
    if (toAdd.length > 0) saveProjects([...existing, ...toAdd]);
    if (!localStorage.getItem(KEYS.notifications)) {
      localStorage.setItem(KEYS.notifications, JSON.stringify([]));
    }
    if (!localStorage.getItem(KEYS.proposals)) {
      localStorage.setItem(KEYS.proposals, JSON.stringify([]));
    }
    if (!localStorage.getItem(KEYS.devProfiles)) {
      localStorage.setItem(KEYS.devProfiles, JSON.stringify(DEMO_PROFILES));
    }
  }

  // ── Projects ──
  function getProjects() { return JSON.parse(localStorage.getItem(KEYS.projects)) || []; }
  function saveProjects(list) { localStorage.setItem(KEYS.projects, JSON.stringify(list)); }

  function addProject(data) {
    const list = getProjects();
    const project = { ...data, id: Date.now(), proposals: 0, createdAt: "agora mesmo", status: "open" };
    list.unshift(project);
    saveProjects(list);
    addNotification(data.ownerEmail, `Projeto "${project.title}" publicado com sucesso!`, "project", project.id);
    return project;
  }

  function getProjectsByUser(email) {
    return getProjects().filter(p => p.ownerEmail === email);
  }

  // ── Proposals ──
  function getProposals() { return JSON.parse(localStorage.getItem(KEYS.proposals)) || []; }
  function saveProposals(list) { localStorage.setItem(KEYS.proposals, JSON.stringify(list)); }

  function getProposalsByProject(projectId) {
    return getProposals().filter(p => String(p.projectId) === String(projectId));
  }

  function getProposalsByDev(devEmail) {
    return getProposals().filter(p => p.devEmail === devEmail);
  }

  function sendProposal({ projectId, devEmail, devName, value, deadline, message }) {
    const proposals = getProposals();
    const already = proposals.find(p => p.projectId === projectId && p.devEmail === devEmail);
    if (already) return { ok: false, msg: "Você já enviou uma proposta para este projeto." };

    const proposal = {
      id: Date.now(),
      projectId,
      devEmail,
      devName,
      value,
      deadline,
      message,
      status: "pending",
      createdAt: new Date().toLocaleString("pt-BR"),
    };
    proposals.push(proposal);
    saveProposals(proposals);

    // increment proposal count on project
    const projects = getProjects();
    const idx = projects.findIndex(p => p.id === projectId);
    if (idx !== -1) { projects[idx].proposals = (projects[idx].proposals || 0) + 1; saveProjects(projects); }

    // notify project owner
    const project = projects.find(p => p.id === projectId);
    if (project && project.ownerEmail) {
      addNotification(project.ownerEmail, `${devName} enviou uma proposta para "${project.title}".`, "proposal", projectId);
    }

    return { ok: true, proposal };
  }

  function acceptProposal(proposalId) {
    const proposals = getProposals();
    const idx = proposals.findIndex(p => p.id === proposalId);
    if (idx === -1) return;

    const proposal = proposals[idx];
    proposals[idx].status = "accepted";

    // reject all others for same project
    proposals.forEach((p, i) => {
      if (p.projectId === proposal.projectId && p.id !== proposalId) proposals[i].status = "rejected";
    });
    saveProposals(proposals);

    // update project status
    const projects = getProjects();
    const pIdx = projects.findIndex(p => p.id === proposal.projectId);
    if (pIdx !== -1) {
      projects[pIdx].status = "in_progress";
      projects[pIdx].acceptedAt = new Date().toLocaleDateString("pt-BR");
      projects[pIdx].acceptedDevEmail = proposal.devEmail;
      projects[pIdx].acceptedDevName = proposal.devName;
      saveProjects(projects);
    }

    const project = projects.find(p => p.id === proposal.projectId);

    // notify the dev whose proposal was accepted
    addNotification(proposal.devEmail, `Sua proposta para "${project ? project.title : "um projeto"}" foi aceita! 🎉`, "accepted", proposal.projectId);

    // notify the client
    if (project && project.ownerEmail) {
      addNotification(project.ownerEmail, `Você aceitou a proposta de ${proposal.devName}. Projeto em andamento!`, "project", proposal.projectId);
    }
  }

  // ── Dev Profiles ──
  function getDevProfiles() { return JSON.parse(localStorage.getItem(KEYS.devProfiles)) || []; }

  function getDevProfile(email) {
    return getDevProfiles().find(p => p.email === email) || null;
  }

  function upsertDevProfile(email, data) {
    const profiles = getDevProfiles();
    const idx = profiles.findIndex(p => p.email === email);
    if (idx === -1) {
      profiles.push({ email, ...data });
    } else {
      profiles[idx] = { ...profiles[idx], ...data };
    }
    localStorage.setItem(KEYS.devProfiles, JSON.stringify(profiles));
  }

  // ── Notifications (per-user) ──
  function _notifKey(email) { return KEYS.notifications + "_" + email; }

  function getNotifications(email) {
    const key = email ? _notifKey(email) : KEYS.notifications;
    return JSON.parse(localStorage.getItem(key)) || [];
  }

  function saveNotifications(email, list) {
    localStorage.setItem(_notifKey(email), JSON.stringify(list));
  }

  function addNotification(email, message, type = "info", projectId = null) {
    const list = getNotifications(email);
    list.unshift({ id: Date.now(), message, type, projectId, read: false, createdAt: new Date().toLocaleString("pt-BR") });
    saveNotifications(email, list);
  }

  function markAllRead(email) {
    const list = getNotifications(email).map(n => ({ ...n, read: true }));
    saveNotifications(email, list);
  }

  function unreadCount(email) { return getNotifications(email).filter(n => !n.read).length; }

  return {
    seed,
    getProjects, addProject, getProjectsByUser,
    getProposalsByProject, getProposalsByDev, sendProposal, acceptProposal,
    getDevProfile, upsertDevProfile,
    getNotifications, addNotification, markAllRead, unreadCount,
  };
})();

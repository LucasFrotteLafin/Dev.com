// ── Configuração da API ────────────────────────────────────────
const API_BASE = "http://localhost:5000";

// ── Sessão (JWT no localStorage) ──────────────────────────────
const Session = {
  save(token, user) {
    localStorage.setItem("dvc_token", token);
    localStorage.setItem("dvc_user", JSON.stringify(user));
  },
  getToken() { return localStorage.getItem("dvc_token"); },
  getUser()  { return JSON.parse(localStorage.getItem("dvc_user")); },
  clear()    { localStorage.removeItem("dvc_token"); localStorage.removeItem("dvc_user"); },
  isLogged() { return !!localStorage.getItem("dvc_token"); },
};

// ── Cliente HTTP ───────────────────────────────────────────────
async function api(path, { method = "GET", body, auth = false } = {}) {
  const headers = { "Content-Type": "application/json" };
  if (auth) {
    const token = Session.getToken();
    if (token) headers["Authorization"] = "Bearer " + token;
  }
  const res = await fetch(API_BASE + path, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  });
  if (res.status === 204) return null;
  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw { status: res.status, message: data.message || "Erro na requisição." };
  return data;
}

// ── Helpers visuais ────────────────────────────────────────────
function getStatusLabel(status) {
  const map = { Open: "Aberto", InProgress: "Em andamento", Completed: "Concluído", Cancelled: "Cancelado" };
  return map[status] ?? status;
}

function getStatusBadge(status) {
  const map = { Open: "badge-green", InProgress: "badge-yellow", Completed: "badge-purple", Cancelled: "badge-red" };
  return map[status] ?? "badge-green";
}

function formatBudget(value) {
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);
}

function formatDate(iso) {
  if (!iso) return "—";
  return new Date(iso).toLocaleDateString("pt-BR");
}

function goTo(page, id) {
  window.location.href = id ? `${page}?id=${id}` : page;
}

// ── Nav dinâmica ───────────────────────────────────────────────
function renderNav() {
  const user = Session.getUser();
  const btn  = document.getElementById("nav-user-btn");
  if (!btn) return;
  if (user) {
    btn.textContent = user.name.split(" ")[0];
    btn.onclick = () => { window.location.href = "dashboard.html"; };
  } else {
    btn.textContent = "Entrar";
    btn.onclick = () => { window.location.href = "auth.html"; };
  }
}

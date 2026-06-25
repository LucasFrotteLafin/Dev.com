// ── Auth ───────────────────────────────────────────────────────
const Auth = (() => {
  const KEYS = { users: "dvc_users", session: "dvc_session" };

  function getUsers() { return JSON.parse(localStorage.getItem(KEYS.users)) || []; }
  function saveUsers(u) { localStorage.setItem(KEYS.users, JSON.stringify(u)); }
  function getSession() { return JSON.parse(localStorage.getItem(KEYS.session)) || null; }
  function setSession(user) { localStorage.setItem(KEYS.session, JSON.stringify(user)); }
  function clearSession() { localStorage.removeItem(KEYS.session); }

  function register({ name, email, password, role }) {
    const users = getUsers();
    if (users.find(u => u.email === email)) return { ok: false, msg: "E-mail já cadastrado." };
    const user = { name, email, password, role, createdAt: new Date().toLocaleDateString("pt-BR") };
    users.push(user);
    saveUsers(users);
    setSession(user);
    return { ok: true, user };
  }

  function login({ email, password }) {
    const user = getUsers().find(u => u.email === email && u.password === password);
    if (!user) return { ok: false, msg: "E-mail ou senha incorretos." };
    setSession(user);
    return { ok: true, user };
  }

  function logout() { clearSession(); window.location.href = "index.html"; }

  function resetPassword({ email, newPassword }) {
    const users = getUsers();
    const idx = users.findIndex(u => u.email === email);
    if (idx === -1) return { ok: false, msg: "E-mail não encontrado." };
    users[idx].password = newPassword;
    saveUsers(users);
    return { ok: true };
  }

  function requireAuth() {
    if (!getSession()) { window.location.href = "auth.html"; return null; }
    return getSession();
  }

  return { register, login, logout, resetPassword, getSession, requireAuth };
})();

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var ts          = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
var emailClient = $"emptest_{ts}@devcom.com";
var emailDev    = $"devtest_{ts}@devcom.com";
var base_url    = "http://localhost:5000";
var http        = new HttpClient { BaseAddress = new Uri(base_url), Timeout = TimeSpan.FromSeconds(15) };
var opts     = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = false };

int passed = 0, failed = 0;
string? tokenClient = null;
string? tokenDev    = null;
int clientUserId    = 0;
int devUserId       = 0;
int projectId       = 0;
int proposalId      = 0;

async Task<(int status, JsonNode? body)> Req(
    string method, string path, object? body = null, string? token = null)
{
    var req = new HttpRequestMessage(new HttpMethod(method), path);
    if (body != null)
        req.Content = new StringContent(
            JsonSerializer.Serialize(body, opts), Encoding.UTF8, "application/json");
    if (token != null)
        req.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    try
    {
        var res  = await http.SendAsync(req);
        var text = await res.Content.ReadAsStringAsync();
        JsonNode? node = null;
        try { node = JsonNode.Parse(text); } catch { }
        return ((int)res.StatusCode, node);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  EXCEPTION: {ex.Message}");
        return (0, null);
    }
}

void Ok(string label, bool condition, string detail = "")
{
    if (condition) { Console.WriteLine($"  ✓ PASS  {label}"); passed++; }
    else           { Console.WriteLine($"  ✗ FAIL  {label} {detail}"); failed++; }
}

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ HEALTH CHECKS ══════════════════════════════════════════");

var (hStatus, hBody) = await Req("GET", "/health");
Ok("/health retorna 200",  hStatus == 200, $"got {hStatus}");
Ok("postgresql Healthy",   hBody?["checks"]?.AsArray().Any(c => c!["name"]!.ToString() == "postgresql" && c!["status"]!.ToString() == "Healthy") == true);
Ok("redis Healthy",        hBody?["checks"]?.AsArray().Any(c => c!["name"]!.ToString() == "redis"      && c!["status"]!.ToString() == "Healthy") == true);

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ AUTH ════════════════════════════════════════════════════");

// Register Client
var (rcs, rcb) = await Req("POST", "/api/auth/register", new {
    name = "Empresa Teste", email = emailClient,
    password = "Test@1234", role = "Client"
});
Ok("Register Client 200",      rcs == 200, $"got {rcs} | {rcb}");
Ok("Register Client has token", rcb?["token"] != null);
tokenClient  = rcb?["token"]?.ToString();
clientUserId = rcb?["user"]?["id"]?.GetValue<int>() ?? 0;
Ok("Register Client role=Client", rcb?["user"]?["role"]?.ToString() == "Client");

// Register Dev
var (rds, rdb) = await Req("POST", "/api/auth/register", new {
    name = "Dev Teste", email = emailDev,
    password = "Test@1234", role = "Dev"
});
Ok("Register Dev 200",       rds == 200, $"got {rds} | {rdb}");
tokenDev  = rdb?["token"]?.ToString();
devUserId = rdb?["user"]?["id"]?.GetValue<int>() ?? 0;
Ok("Register Dev role=Dev",  rdb?["user"]?["role"]?.ToString() == "Dev");

// Login Client
var (lcs, lcb) = await Req("POST", "/api/auth/login", new {
    email = emailClient, password = "Test@1234"
});
Ok("Login Client 200",      lcs == 200, $"got {lcs}");
Ok("Login Client has token", lcb?["token"] != null);
tokenClient = lcb?["token"]?.ToString() ?? tokenClient;

// Login Dev
var (lds, ldb) = await Req("POST", "/api/auth/login", new {
    email = emailDev, password = "Test@1234"
});
Ok("Login Dev 200",      lds == 200, $"got {lds}");
tokenDev = ldb?["token"]?.ToString() ?? tokenDev;

// Login errado
var (les, _) = await Req("POST", "/api/auth/login", new {
    email = emailClient, password = "SenhaErrada"
});
Ok("Login senha errada retorna 403", les == 403, $"got {les}");

// Reset password
var (rps, _) = await Req("POST", "/api/auth/reset-password", new {
    email = emailClient, newPassword = "Test@1234"
});
Ok("Reset password 200", rps == 200, $"got {rps}");

// Email duplicado
var (dup, _) = await Req("POST", "/api/auth/register", new {
    name = "Outro", email = emailClient, password = "Test@1234", role = "Client"
});
Ok("Email duplicado retorna 400", dup == 400, $"got {dup}");

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ PROJECTS ════════════════════════════════════════════════");

// GET paginado (sem auth)
var (ps, pb) = await Req("GET", "/api/projects?page=1&pageSize=5");
Ok("GET /projects 200",          ps == 200, $"got {ps}");
Ok("GET /projects tem items",    pb?["items"] != null);
Ok("GET /projects totalCount>0", pb?["totalCount"]?.GetValue<int>() > 0);
Ok("GET /projects totalPages>0", pb?["totalPages"]?.GetValue<int>() > 0);
Ok("GET /projects page=1",       pb?["page"]?.GetValue<int>() == 1);
Ok("GET /projects pageSize=5",   pb?["pageSize"]?.GetValue<int>() == 5);
Ok("GET /projects items<=5",     pb?["items"]?.AsArray().Count <= 5);

// GET filtro status=Open
var (pos, pob) = await Req("GET", "/api/projects?status=Open&pageSize=3");
Ok("GET /projects?status=Open 200", pos == 200, $"got {pos}");
var openItems = pob?["items"]?.AsArray();
Ok("Todos items status=Open",
    openItems?.All(i => i!["status"]?.ToString() == "Open") == true);

// GET filtro category
var (pcs, pcb) = await Req("GET", "/api/projects?category=Dashboard");
Ok("GET /projects?category=Dashboard 200", pcs == 200, $"got {pcs}");

// GET por ID
var (pid1s, pid1b) = await Req("GET", "/api/projects/1");
Ok("GET /projects/1 200",         pid1s == 200, $"got {pid1s}");
Ok("GET /projects/1 tem title",   pid1b?["title"] != null);
Ok("GET /projects/1 tem tags",    pid1b?["tags"] != null);
Ok("GET /projects/1 tem ownerId", pid1b?["ownerId"] != null);

// GET ID inexistente
var (pid999s, _) = await Req("GET", "/api/projects/999");
Ok("GET /projects/999 retorna 404", pid999s == 404, $"got {pid999s}");

// GET /projects sem auth retorna 401
var (pmys, _) = await Req("GET", "/api/projects/my");
Ok("GET /projects/my sem auth retorna 401", pmys == 401, $"got {pmys}");

// GET /projects/my com auth (Client)
var (pmycs, pmycb) = await Req("GET", "/api/projects/my", token: tokenClient);
Ok("GET /projects/my Client 200", pmycs == 200, $"got {pmycs}");

// POST criar projeto (Client)
var (pcrS, pcrB) = await Req("POST", "/api/projects", new {
    title       = "Projeto de Teste",
    description = "Descricao bem longa do projeto de teste para validar endpoint",
    budget      = 2500.00,
    deadline    = "15 dias",
    category    = "Web App",
    tags        = new[] { "React", "Node.js" }
}, tokenClient);
Ok("POST /projects Client 201",    pcrS == 201, $"got {pcrS} | {pcrB}");
projectId = pcrB?["id"]?.GetValue<int>() ?? 0;
Ok("POST /projects retornou id>0", projectId > 0, $"id={projectId}");

// POST criar projeto sem auth
var (pcrNoAuth, _) = await Req("POST", "/api/projects", new {
    title = "X", description = "Y", budget = 100, deadline = "1 dia", category = "Outro"
});
Ok("POST /projects sem auth retorna 401", pcrNoAuth == 401, $"got {pcrNoAuth}");

// POST criar projeto com Dev (deve falhar)
var (pcrDev, _) = await Req("POST", "/api/projects", new {
    title       = "Dev nao pode criar",
    description = "Descricao longa o suficiente para passar na validacao",
    budget      = 1000, deadline = "10 dias", category = "Outro"
}, tokenDev);
Ok("POST /projects com Dev retorna 403", pcrDev == 403, $"got {pcrDev}");

// Cache — segunda chamada deve ser mais rápida (apenas verifica que funciona)
var (pc2s, _) = await Req("GET", $"/api/projects/{projectId}");
Ok($"GET /projects/{projectId} cache hit 200", pc2s == 200, $"got {pc2s}");

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ PROPOSALS ═══════════════════════════════════════════════");

// Dev envia proposta
var (prS, prB) = await Req("POST", $"/api/projects/{projectId}/proposals", new {
    value    = "R$ 2.000",
    deadline = "10 dias",
    message  = "Tenho experiência nessa área e posso entregar com qualidade."
}, tokenDev);
Ok("POST /proposals Dev 200",        prS == 200, $"got {prS} | {prB}");
proposalId = prB?["id"]?.GetValue<int>() ?? 0;
Ok("POST /proposals retornou id>0",  proposalId > 0, $"id={proposalId}");
Ok("POST /proposals status=Pending", prB?["status"]?.ToString() == "Pending");

// Dev envia proposta duplicada (deve falhar)
var (prDup, _) = await Req("POST", $"/api/projects/{projectId}/proposals", new {
    value = "R$ 1.000", deadline = "5 dias",
    message = "Segunda tentativa que deve ser bloqueada pelo sistema."
}, tokenDev);
Ok("Proposta duplicada retorna 400", prDup == 400, $"got {prDup}");

// Client tenta enviar proposta (deve falhar)
var (prClient, _) = await Req("POST", $"/api/projects/{projectId}/proposals", new {
    value = "R$ 500", deadline = "3 dias",
    message = "Cliente tentando enviar proposta deve ser bloqueado."
}, tokenClient);
Ok("Client envia proposta retorna 403", prClient == 403, $"got {prClient}");

// GET propostas do projeto (como Client dono)
var (prListS, prListB) = await Req("GET", $"/api/projects/{projectId}/proposals", token: tokenClient);
Ok("GET /proposals Client dono 200",    prListS == 200, $"got {prListS}");
Ok("GET /proposals tem 1 proposta",     prListB?.AsArray().Count == 1);

// GET propostas sem auth
var (prNoAuth, _) = await Req("GET", $"/api/projects/{projectId}/proposals");
Ok("GET /proposals sem auth 401",       prNoAuth == 401, $"got {prNoAuth}");

// GET /proposals/my (Dev)
var (prMyS, prMyB) = await Req("GET", "/api/proposals/my", token: tokenDev);
Ok("GET /proposals/my Dev 200",         prMyS == 200, $"got {prMyS}");
Ok("GET /proposals/my tem projectTitle", prMyB?.AsArray().Any(p => p!["projectTitle"] != null) == true);

// PATCH aceitar proposta (Client dono)
var (accS, accB) = await Req("PATCH", $"/api/proposals/{proposalId}/accept", token: tokenClient);
Ok("PATCH /accept Client dono 200",     accS == 200, $"got {accS} | {accB}");

// Verifica projeto mudou para InProgress
var (pAfterS, pAfterB) = await Req("GET", $"/api/projects/{projectId}");
Ok("Projeto status=InProgress após aceite", pAfterB?["status"]?.ToString() == "InProgress", $"got {pAfterB?["status"]}");

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ NOTIFICATIONS ════════════════════════════════════════════");

// GET notifications Client (deve ter 2: proposta enviada + aceite confirmado)
var (notS, notB) = await Req("GET", "/api/notifications", token: tokenClient);
Ok("GET /notifications Client 200",    notS == 200, $"got {notS}");
Ok("GET /notifications Client count>0", notB?.AsArray().Count > 0);

// GET notifications Dev (deve ter 1: proposta aceita)
var (notDS, notDB) = await Req("GET", "/api/notifications", token: tokenDev);
Ok("GET /notifications Dev 200",       notDS == 200, $"got {notDS}");
Ok("GET /notifications Dev count>0",   notDB?.AsArray().Count > 0);

// GET unread-count
var (uncS, uncB) = await Req("GET", "/api/notifications/unread-count", token: tokenClient);
Ok("GET /unread-count 200",            uncS == 200, $"got {uncS}");
Ok("GET /unread-count count>=0",       uncB?["count"]?.GetValue<int>() >= 0);

// PATCH mark all read
var (marS, _) = await Req("PATCH", "/api/notifications/read-all", token: tokenClient);
Ok("PATCH /read-all 204",              marS == 204, $"got {marS}");

// Verifica que count zerou
var (uncAfterS, uncAfterB) = await Req("GET", "/api/notifications/unread-count", token: tokenClient);
Ok("unread-count=0 após mark all read", uncAfterB?["count"]?.GetValue<int>() == 0, $"got {uncAfterB?["count"]}");

// Sem auth deve retornar 401
var (notNoAuth, _) = await Req("GET", "/api/notifications");
Ok("GET /notifications sem auth 401",  notNoAuth == 401, $"got {notNoAuth}");

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ DEV PROFILE ══════════════════════════════════════════════");

// Perfil do Pedro (seed)
var (dpS, dpB) = await Req("GET", "/api/devs/pedro%40dev.com");
Ok("GET /devs/:email 200",          dpS == 200, $"got {dpS}");
Ok("GET /devs tem name",            dpB?["name"]?.ToString() == "Pedro Costa");
Ok("GET /devs tem skills",          dpB?["skills"]?.AsArray().Count > 0);
Ok("GET /devs tem feedbacks",       dpB?["feedbacks"]?.AsArray().Count > 0);
Ok("GET /devs tem rating>0",        dpB?["rating"]?.GetValue<double>() > 0);

// Email inexistente
var (dpNotS, _) = await Req("GET", "/api/devs/naoeexiste%40nada.com");
Ok("GET /devs email inexistente 404", dpNotS == 404, $"got {dpNotS}");

// Client não tem perfil de dev
var (dpClientS, _) = await Req("GET", $"/api/devs/{Uri.EscapeDataString(emailClient)}");
Ok("GET /devs com email de Client 404", dpClientS == 404, $"got {dpClientS}");

// ════════════════════════════════════════════════════════════════
Console.WriteLine("\n══ RESULTADO FINAL ══════════════════════════════════════════");
Console.WriteLine($"  Total: {passed + failed} | PASS: {passed} | FAIL: {failed}");
if (failed == 0)
    Console.WriteLine("  🎉 Todos os endpoints funcionando corretamente!");
else
    Console.WriteLine($"  ⚠️  {failed} teste(s) falharam — veja detalhes acima.");

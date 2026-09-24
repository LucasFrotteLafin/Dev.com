using System.Text;
using DevCom.API.Middleware;
using DevCom.Application;
using DevCom.Infrastructure;
using DevCom.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Enums como string ("Client", "Dev", "Open", etc.)
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        // camelCase para o frontend (token, user, items, totalCount...)
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ── CORS ──────────────────────────────────────────────────────────────────────
// SetIsOriginAllowed permite file:// (origin "null") e localhost em qualquer porta
builder.Services.AddCors(opts => opts.AddDefaultPolicy(policy =>
    policy
        .SetIsOriginAllowed(origin =>
        {
            if (origin == "null") return true; // file://
            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return uri.Host == "localhost" || uri.Host == "127.0.0.1";
            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

// ── Camadas de aplicacao ──────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── JWT ───────────────────────────────────────────────────────────────────────
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            // Mapeia o claim "sub" para ClaimTypes.NameIdentifier
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };

        // Retorna 401 com JSON em vez de HTML
        opts.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode  = 401;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsync("{\"message\":\"Nao autenticado.\"}");
            },
            OnForbidden = async ctx =>
            {
                ctx.Response.StatusCode  = 403;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsync("{\"message\":\"Acesso negado.\"}");
            }
        };
    });

builder.Services.AddAuthorization();

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db" })
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "cache" });

// ── Swagger com autenticacao Bearer ──────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "DevCom API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Informe: Bearer {token}"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// ═════════════════════════════════════════════════════════════════════════════
var app = builder.Build();

// ── Migrations automaticas na inicializacao ───────────────────────────────────
await ApplyMigrationsAsync(app);

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Health Check endpoints ─────────────────────────────────────────────────────
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new
        {
            status        = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds + "ms",
            checks        = report.Entries.Select(e => new
            {
                name        = e.Key,
                status      = e.Value.Status.ToString(),
                duration    = e.Value.Duration.TotalMilliseconds + "ms",
                description = e.Value.Description
            })
        });
    }
});

app.MapHealthChecks("/health/db",    new HealthCheckOptions { Predicate = h => h.Tags.Contains("db") });
app.MapHealthChecks("/health/cache", new HealthCheckOptions { Predicate = h => h.Tags.Contains("cache") });

app.Run();

// ── helper ────────────────────────────────────────────────────────────────────
static async Task ApplyMigrationsAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        app.Logger.LogInformation("Migrations aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao aplicar migrations. A API continuara sem migrar.");
    }
}

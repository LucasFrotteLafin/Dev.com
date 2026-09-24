using DevCom.Application.Interfaces;
using DevCom.Infrastructure.Cache;
using DevCom.Infrastructure.Data;
using DevCom.Infrastructure.Repositories;
using DevCom.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DevCom.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Banco de dados ────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // ── Redis — lazy connect, nao bloqueia startup se Redis nao estiver up ──
        var redisConn = configuration.GetConnectionString("Redis")!;

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var configOpts = ConfigurationOptions.Parse(redisConn);
            configOpts.AbortOnConnectFail = false;
            configOpts.ConnectRetry       = 3;
            configOpts.ReconnectRetryPolicy = new ExponentialRetry(5000);
            return ConnectionMultiplexer.Connect(configOpts);
        });

        services.AddStackExchangeRedisCache(opts =>
            opts.Configuration = redisConn);

        services.AddScoped<ICacheService, RedisCacheService>();

        // ── Seguranca ─────────────────────────────────────────────────────────
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService,   JwtTokenService>();

        // ── Repositorios ──────────────────────────────────────────────────────
        services.AddScoped<IUserRepository,         UserRepository>();
        services.AddScoped<IProjectRepository,      ProjectRepository>();
        services.AddScoped<IProposalRepository,     ProposalRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IFeedbackRepository,     FeedbackRepository>();

        return services;
    }
}

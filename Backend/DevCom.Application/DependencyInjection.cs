using DevCom.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevCom.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<ProposalService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<DevProfileService>();
        return services;
    }
}

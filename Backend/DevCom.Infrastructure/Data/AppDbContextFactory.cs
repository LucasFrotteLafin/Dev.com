using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevCom.Infrastructure.Data;

/// <summary>
/// Usada pelo 'dotnet ef migrations' em design-time.
/// Evita que o EF CLI inicialize Redis ou outros serviços externos.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=Dev.com;Username=postgres;Password=240505")
            .Options;

        return new AppDbContext(options);
    }
}

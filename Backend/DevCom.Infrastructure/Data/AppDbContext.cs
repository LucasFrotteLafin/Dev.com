using DevCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevCom.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User>         Users         => Set<User>();
    public DbSet<Project>      Projects      => Set<Project>();
    public DbSet<ProjectTag>   ProjectTags   => Set<ProjectTag>();
    public DbSet<Proposal>     Proposals     => Set<Proposal>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Feedback>     Feedbacks     => Set<Feedback>();
    public DbSet<DevSkill>     DevSkills     => Set<DevSkill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

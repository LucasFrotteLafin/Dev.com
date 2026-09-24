using DevCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevCom.Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(300);
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Budget).IsRequired().HasColumnType("numeric(18,2)");
        builder.Property(p => p.Deadline).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Status).HasConversion<string>().IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();

        // FK para dev aceito sem cascade (evita ciclo com OwnedProjects)
        builder.HasOne(p => p.AcceptedDev)
               .WithMany()
               .HasForeignKey(p => p.AcceptedDevId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Tags)
               .WithOne(t => t.Project)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Proposals)
               .WithOne(pr => pr.Project)
               .HasForeignKey(pr => pr.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

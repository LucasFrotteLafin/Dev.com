using DevCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevCom.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().IsRequired();
        builder.Property(u => u.Bio).HasMaxLength(1000);
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.HasMany(u => u.OwnedProjects)
               .WithOne(p => p.Owner)
               .HasForeignKey(p => p.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SentProposals)
               .WithOne(p => p.Dev)
               .HasForeignKey(p => p.DevId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notifications)
               .WithOne(n => n.User)
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ReceivedFeedbacks)
               .WithOne(f => f.Dev)
               .HasForeignKey(f => f.DevId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Skills)
               .WithOne(s => s.Dev)
               .HasForeignKey(s => s.DevId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

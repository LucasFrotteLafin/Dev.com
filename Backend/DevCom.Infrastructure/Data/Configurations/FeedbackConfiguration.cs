using DevCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevCom.Infrastructure.Data.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("feedbacks");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Stars).IsRequired();
        builder.Property(f => f.Text).IsRequired().HasMaxLength(1000);
        builder.Property(f => f.Company).IsRequired().HasMaxLength(200);
        builder.Property(f => f.CreatedAt).IsRequired();

        builder.HasOne(f => f.Project)
               .WithMany()
               .HasForeignKey(f => f.ProjectId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

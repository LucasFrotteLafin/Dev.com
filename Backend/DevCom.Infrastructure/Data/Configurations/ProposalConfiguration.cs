using DevCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevCom.Infrastructure.Data.Configurations;

public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.ToTable("proposals");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Value).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Deadline).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Message).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();

        // Garante que um dev não envie duas propostas para o mesmo projeto
        builder.HasIndex(p => new { p.ProjectId, p.DevId }).IsUnique();
    }
}

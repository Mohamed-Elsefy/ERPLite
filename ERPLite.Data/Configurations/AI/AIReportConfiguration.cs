using ERPLite.Data.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.AI
{
    public class AIReportConfiguration : IEntityTypeConfiguration<AIReport>
    {
        public void Configure(EntityTypeBuilder<AIReport> builder)
        {
            builder.ToTable("AIReports");

            builder.HasKey(ar => ar.Id);

            builder.Property(ar => ar.Type)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(ar => ar.InputSummary)
                .IsRequired();

            builder.Property(ar => ar.AIResponse)
                .IsRequired();

            builder.Property(ar => ar.RelatedEntityType)
                .HasMaxLength(50);

            builder.Property(ar => ar.CreatedAt)
                .IsRequired();

            builder.HasOne(ar => ar.GeneratedByUser)
                .WithMany()
                .HasForeignKey(ar => ar.GeneratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
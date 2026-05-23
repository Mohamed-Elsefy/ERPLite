using ERPLite.Data.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.AI
{
    public class AILogConfiguration
        : IEntityTypeConfiguration<AILog>
    {
        public void Configure(EntityTypeBuilder<AILog> builder)
        {
            builder.ToTable("AILogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Prompt)
                .IsRequired();

            builder.Property(al => al.Response)
                .IsRequired();

            builder.Property(al => al.TokensUsed)
                .IsRequired();

            builder.Property(al => al.CreatedAt)
                .IsRequired();
        }
    }
}
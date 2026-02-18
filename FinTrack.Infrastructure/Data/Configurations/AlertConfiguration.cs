using FinTrack.Core.Entities;
using FinTrack.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("Alerts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Message)
                .HasMaxLength(500)
                .IsRequired();

            // Enums como strings
            builder.Property(a => a.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Severity)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.IsRead)
                .HasDefaultValue(false);

            builder.Property(a => a.RelatedEntityType)
                .HasMaxLength(50)
                .IsRequired(false);

            // Índices
            builder.HasIndex(a => new { a.UserId, a.IsRead, a.CreatedAt })
                .HasDatabaseName("IX_Alerts_UserId_IsRead_CreatedAt");

            builder.HasIndex(a => a.Type)
                .HasDatabaseName("IX_Alerts_Type");

            // Relaciones
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
using FinTrack.Core.Entities;
using FinTrack.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
    {
        public void Configure(EntityTypeBuilder<Prediction> builder)
        {
            builder.ToTable("Predictions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PredictedAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Confidence)
                .HasPrecision(5, 2); // 0.00 a 100.00

            builder.Property(p => p.PredictionDate)
                .IsRequired();

            builder.Property(p => p.GeneratedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // Índices
            builder.HasIndex(p => new { p.UserId, p.PredictionDate })
                .HasDatabaseName("IX_Predictions_UserId_PredictionDate");

            builder.HasIndex(p => p.GeneratedDate)
                .HasDatabaseName("IX_Predictions_GeneratedDate");

            // Relaciones
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false)
                .HasConstraintName("FK_Predictions_Categories_CategoryId");
        }
    }
}

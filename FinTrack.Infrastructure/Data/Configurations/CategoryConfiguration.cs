using FinTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.Color)
                .HasMaxLength(7)
                .HasDefaultValue("#3498db");

            // Enum como string
            builder.Property(c => c.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Índice compuesto para búsquedas rápidas
            builder.HasIndex(c => new { c.UserId, c.Type })
                .HasDatabaseName("IX_Categories_UserId_Type");

            builder.HasIndex(c => c.Name)
                .HasDatabaseName("IX_Categories_Name");

            // Relación con User
            builder.HasOne<Core.Entities.User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Categories_AspNetUsers_UserId");

            // Restricción: Nombre único por usuario
            builder.HasIndex(c => new { c.UserId, c.Name })
                .IsUnique()
                .HasDatabaseName("UK_Categories_UserId_Name");
        }
    }
}
using FinTrack.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configuraciones personalizadas para IdentityUser

            builder.Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Índice para búsqueda por nombre completo
            builder.HasIndex(u => new { u.FirstName, u.LastName })
                .HasDatabaseName("IX_AspNetUsers_Name");

            // Valor por defecto para email confirmado (para desarrollo)
            builder.Property(u => u.EmailConfirmed)
                .HasDefaultValue(true);
        }
    }
}

using FinTrack.Core.Entities;
using FinTrack.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class IncomeConfiguration : IEntityTypeConfiguration<Income>
    {
        public void Configure(EntityTypeBuilder<Income> builder)
        {
            builder.ToTable("Incomes");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(i => i.Description)
                .HasMaxLength(200);

            builder.Property(i => i.Date)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Índices
            builder.HasIndex(i => new { i.UserId, i.Date })
                .HasDatabaseName("IX_Incomes_UserId_Date");

            builder.HasIndex(i => i.CategoryId)
                .HasDatabaseName("IX_Incomes_CategoryId");

            // Relaciones
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(i => i.Category)
                .WithMany(c => c.Incomes)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Incomes_Categories_CategoryId");
        }
    }
}

using FinTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.ToTable("Budgets");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(b => b.StartDate)
                .IsRequired();

            builder.Property(b => b.EndDate)
                .IsRequired();

            // Validación en BD: EndDate > StartDate
            builder.HasCheckConstraint(
                "CK_Budget_Dates",
                "[EndDate] > [StartDate]");

            // Validación en BD: Amount > 0
            builder.HasCheckConstraint(
                "CK_Budget_Amount",
                "[Amount] > 0");

            // Índices
            builder.HasIndex(b => new { b.UserId, b.StartDate, b.EndDate })
                .HasDatabaseName("IX_Budgets_UserId_Dates");

            builder.HasIndex(b => b.CategoryId)
                .HasDatabaseName("IX_Budgets_CategoryId");

            // Relaciones
            builder.HasOne<Core.Entities.User>()
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Budgets_AspNetUsers_UserId");

            builder.HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false)
                .HasConstraintName("FK_Budgets_Categories_CategoryId");
        }
    }
}
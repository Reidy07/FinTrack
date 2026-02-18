using FinTrack.Core.Entities;
using FinTrack.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            // Tabla
            builder.ToTable("Expenses");

            // Clave primaria
            builder.HasKey(e => e.Id);

            // Propiedades
            builder.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(200);

            builder.Property(e => e.Date)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.IsRecurring)
                .HasDefaultValue(false);

            builder.Property(e => e.RecurringPattern)
                .HasMaxLength(50)
                .IsRequired(false);

            // Índices
            builder.HasIndex(e => new { e.UserId, e.Date })
                .HasDatabaseName("IX_Expenses_UserId_Date");

            builder.HasIndex(e => e.CategoryId)
                .HasDatabaseName("IX_Expenses_CategoryId");

            // Relaciones
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Expenses_Categories_CategoryId");
        }
    }
}
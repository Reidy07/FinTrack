using FinTrack.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static void ConfigureCascadeBehavior(this ModelBuilder builder)
        {
            // Deshabilitar cascada por defecto para todas las relaciones
            var cascadeFKs = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership &&
                             fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Permitir cascada solo para relaciones específicas
            // (como Expenses → User, Incomes → User)
            builder.Entity<Core.Entities.Expense>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Core.Entities.Income>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

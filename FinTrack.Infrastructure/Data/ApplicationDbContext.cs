using FinTrack.Core.Entities;
using FinTrack.Infrastructure.Extensions;
using FinTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FinTrack.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        // No se necesita DbSet<User> porque User es una entidad de Core que no se persiste directamente.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Llamar al método base para configurar Identity
            base.OnModelCreating(builder);

            // Aplicar TODAS las configuraciones del ensamblado actual
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.ConfigureCascadeBehavior();

            // Configuraciones adicionales globales
            ConfigureGlobalProperties(builder);
        }

        private void ConfigureGlobalProperties(ModelBuilder builder)
        {
            // Configuración global para decimales
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal))
                    {
                        property.SetPrecision(18);
                        property.SetScale(2);
                    }
                }
            }
        }

    }
}

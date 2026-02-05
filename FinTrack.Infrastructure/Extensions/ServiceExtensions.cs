using FinTrack.Core.Interfaces;
using FinTrack.Core.Services;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace FinTrack.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)

        {
            // Configurar DbContext (ya se hace en Program.cs, pero por si acaso)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IFinancialService, FinancialService>();
            

            return services;
        }
    }
}
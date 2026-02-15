using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;
using FinTrack.Core.Services;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Repositories;
using FinTrack.Infrastructure.Services;
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
            // 1. Configurar DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FinTrack.Infrastructure")));

            // 2. Registrar UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Registrar Repositorios individuales
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            // 4. Registrar Servicios de Dominio
            services.AddScoped<IFinancialService, FinancialService>();
            services.AddScoped<IPredictionService, PredictionService>();

            return services;
        }
    }
}
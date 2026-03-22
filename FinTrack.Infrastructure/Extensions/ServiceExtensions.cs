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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FinTrack.Infrastructure")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IFinancialService, FinancialService>();
            services.AddScoped<IPredictionService, PredictionService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IGeminiPredictionService, GeminiPredictionService>();
            services.AddScoped<IAIChatbotService, AIChatbotService>();

            return services;
        }
    }
}
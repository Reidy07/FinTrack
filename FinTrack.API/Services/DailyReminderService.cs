using FinTrack.Core.Entities;
using FinTrack.Core.Enum;
using FinTrack.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinTrack.API.Services
{
    // BackgroundService se ejecuta en segundo plano mientras la API esté encendida
    public class DailyReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyReminderService> _logger;

        public DailyReminderService(IServiceProvider serviceProvider, ILogger<DailyReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Ejecutando revisión de alertas diarias...");

                try
                {
                    // Como BackgroundService es Singleton, necesitamos crear un "Scope" para usar la BD
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Nota: En un caso real, buscaríamos solo los usuarios activos. 
                    // Para tu tesis, traeremos todos los usuarios que hayan registrado alguna vez una categoría.
                    var categories = await unitOfWork.Categories.GetAllAsync();
                    var userIds = categories.Select(c => c.UserId).Distinct().ToList();

                    var today = DateTime.UtcNow.Date;

                    foreach (var userId in userIds)
                    {
                        // Verificamos si ingresó gastos HOY
                        var todayExpenses = await unitOfWork.Expenses.FindAsync(e =>
                            e.UserId == userId &&
                            e.Date.Date == today);

                        if (!todayExpenses.Any())
                        {
                            // Revisamos si ya le enviamos esta alerta hoy para no duplicarla
                            var existingAlert = await unitOfWork.Alerts.FindAsync(a =>
                                a.UserId == userId &&
                                a.Type == AlertType.SystemAlert &&
                                a.CreatedAt.Date == today);

                            if (!existingAlert.Any())
                            {
                                var alert = new Alert
                                {
                                    UserId = userId,
                                    Title = "¡No olvides tus registros!",
                                    Message = "¿Ya ingresaste tus gastos de hoy? Mantener tus finanzas al día te ayuda a cumplir tus metas.",
                                    Type = AlertType.SystemAlert,
                                    Severity = AlertSeverity.Info,
                                    CreatedAt = DateTime.UtcNow,
                                    IsRead = false
                                };

                                await unitOfWork.Alerts.AddAsync(alert);
                            }
                        }
                    }

                    await unitOfWork.CompleteAsync();
                    _logger.LogInformation("Revisión de alertas finalizada con éxito.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al generar alertas diarias.");
                }

                // Esperamos 24 horas para volver a revisar
                // (Para pruebas en la explicacion de tesis, podemos ponerlo en 1 minuto: TimeSpan.FromMinutes(1))
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
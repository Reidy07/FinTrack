using FinTrack.Core.Entities;
using FinTrack.Core.Enum;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;

namespace FinTrack.API.Services
{
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
                    // Creamos el scope manualmente
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Traemos nuestros nuevos servicios de correo
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    // Usuarios que han registrado categorías alguna vez
                    var categories = await unitOfWork.Categories.GetAllAsync();
                    var userIds = categories.Select(c => c.UserId).Distinct().ToList();

                    var today = DateTime.UtcNow.Date;

                    foreach (var userId in userIds)
                    {
                        // Verificamos si ingresó gastos hoy
                        var todayExpenses = await unitOfWork.Expenses.FindAsync(e =>
                            e.UserId == userId &&
                            e.Date.Date == today);

                        if (!todayExpenses.Any())
                        {
                            // Revisamos si ya le enviamos esta alerta hoy
                            var existingAlert = await unitOfWork.Alerts.FindAsync(a =>
                                a.UserId == userId &&
                                a.Type == AlertType.SystemAlert &&
                                a.CreatedAt.Date == today);

                            if (!existingAlert.Any())
                            {
                                var alertTitle = "¡No olvides tus registros!";
                                var alertMessage = "¿Ya ingresaste tus gastos de hoy? Mantener tus finanzas al día te ayuda a cumplir tus metas.";

                                var alert = new Alert
                                {
                                    UserId = userId,
                                    Title = alertTitle,
                                    Message = alertMessage,
                                    Type = AlertType.SystemAlert,
                                    Severity = AlertSeverity.Info,
                                    CreatedAt = DateTime.UtcNow,
                                    IsRead = false
                                };

                                await unitOfWork.Alerts.AddAsync(alert);
                                await unitOfWork.CompleteAsync();

                                // Enviar el correo usando la plantilla maestra
                                var userEmail = await userService.GetUserEmailAsync(userId);
                                if (!string.IsNullOrEmpty(userEmail))
                                {
                                    var alertHtml = await emailTemplateService.GetAlertTemplateAsync(
                                        title: alertTitle,
                                        message: alertMessage,
                                        actionUrl: "https://localhost:7127/Expense/Create", // Lo mandamos directo a crear el gasto
                                        actionText: "Registrar gasto de hoy");

                                    await emailService.SendEmailAsync(userEmail, alertTitle, alertHtml);
                                }
                            }
                        }
                    }

                    _logger.LogInformation("Revisión de alertas finalizada con éxito.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al generar alertas diarias.");
                }

                // Para producción sería 1 día: TimeSpan.FromDays(1)
                // Para prueba de Tesis se dejará en 1 minuto
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
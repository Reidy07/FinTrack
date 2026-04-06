using FinTrack.Core.Interfaces.Services;
using System.IO;

namespace FinTrack.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly string _templatesPath;

        public EmailTemplateService()
        {
            // Apunta automáticamente a la carpeta de ejecución donde están nuestros HTML
            _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
        }

        public async Task<string> GetMasterTemplateAsync(string subject, string body)
        {
            var filePath = Path.Combine(_templatesPath, "MasterTemplate.html");
            if (!File.Exists(filePath)) return body; // Si falla, manda el texto plano

            var template = await File.ReadAllTextAsync(filePath);
            return template.Replace("{{Subject}}", subject)
                           .Replace("{{Body}}", body);
        }

        public async Task<string> GetAlertTemplateAsync(string title, string message, string actionUrl, string actionText)
        {
            var filePath = Path.Combine(_templatesPath, "AlertTemplate.html");
            if (!File.Exists(filePath)) return $"{title}: {message} - {actionUrl}";

            var template = await File.ReadAllTextAsync(filePath);
            return template.Replace("{{AlertTitle}}", title)
                           .Replace("{{AlertMessage}}", message)
                           .Replace("{{ActionUrl}}", actionUrl)
                           .Replace("{{ActionText}}", actionText);
        }
    }
}
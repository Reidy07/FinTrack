namespace FinTrack.Core.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        Task<string> GetMasterTemplateAsync(string subject, string body);
        Task<string> GetAlertTemplateAsync(string title, string message, string actionUrl, string actionText);
    }
}

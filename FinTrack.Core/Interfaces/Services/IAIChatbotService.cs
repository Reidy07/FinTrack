namespace FinTrack.Core.Interfaces.Services
{
    public interface IAIChatbotService
    {
        Task<string> AskFinancialAdvisorAsync(string userId, string userMessage);
    }
}
namespace FinTrack.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<string> GetUserEmailAsync(string userId);
    }
}
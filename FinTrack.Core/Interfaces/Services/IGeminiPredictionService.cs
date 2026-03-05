using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTrack.Core.DTOs;

namespace FinTrack.Core.Interfaces.Services
{
    public interface IGeminiPredictionService
    {
        Task<GeminiPredictionResultDto> GetFinancialPredictionAsync(
            string userId,
            List<string> userPriorities,
            decimal? savingsGoalPercentage = null);
    }
}

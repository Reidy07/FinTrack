using FinTrack.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Core.Services
{
    public interface IFinancialService
    {
        Task<IEnumerable<ExpenseDto>> GetExpensesAsync(string userId);
        Task AddExpenseAsync(ExpenseDto dto);
    }
}

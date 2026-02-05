using FinTrack.Core.DTOs;
using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinTrack.Core.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FinancialService(
            IExpenseRepository expenseRepository,
            IUnitOfWork unitOfWork)
        {
            _expenseRepository = expenseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesAsync(string userId)
        {
            var expenses = await _expenseRepository.GetByUserAsync(userId);

            return expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                Date = e.Date,
                CategoryId = e.CategoryId,
                UserId = e.UserId
            });
        }

        public async Task AddExpenseAsync(ExpenseDto dto)
        {
            var expense = new Expense
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId
            };

            await _expenseRepository.AddAsync(expense);
            await _unitOfWork.CompleteAsync();
        }
    }
}

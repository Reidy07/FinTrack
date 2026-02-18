using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositories
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Expense>> GetByUserAsync(string userId)
        {
            return await _dbSet
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }
    }
}

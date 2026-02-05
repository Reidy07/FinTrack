using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinTrack.Infrastructure.Repositories
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetByUserAsync(string userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }
    }
}
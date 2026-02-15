using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using FinTrack.Infrastructure.Repositories;

namespace FinTrack.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Expenses = new GenericRepository<Expense>(_context);
            Incomes = new GenericRepository<Income>(_context);
            Categories = new GenericRepository<Category>(_context);
            Budgets = new GenericRepository<Budget>(_context);
            Predictions = new GenericRepository<Prediction>(_context);
            Alerts = new GenericRepository<Alert>(_context);
        }

        public IGenericRepository<Expense> Expenses { get; }
        public IGenericRepository<Income> Incomes { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Budget> Budgets { get; }
        public IGenericRepository<Prediction> Predictions { get; }
        public IGenericRepository<Alert> Alerts { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
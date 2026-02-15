using FinTrack.Core.Entities;

namespace FinTrack.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Agregamos los repositorios genéricos
        IGenericRepository<Expense> Expenses { get; }
        IGenericRepository<Income> Incomes { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Budget> Budgets { get; }
        IGenericRepository<Prediction> Predictions { get; }
        IGenericRepository<Alert> Alerts { get; }

        Task<int> CompleteAsync();
    }
}

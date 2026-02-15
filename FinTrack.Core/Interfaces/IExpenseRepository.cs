using FinTrack.Core.Entities;


namespace FinTrack.Core.Interfaces
{
   
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetByUserAsync(string userId);
    }

}

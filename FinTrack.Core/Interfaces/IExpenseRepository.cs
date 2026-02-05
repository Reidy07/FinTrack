using FinTrack.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Core.Interfaces
{
   
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetByUserAsync(string userId);
    }

}

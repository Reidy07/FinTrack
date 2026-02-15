using FinTrack.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FinTrack.Tests
{
    public class BalanceCalculationTests
    {
        [Fact]
        public void Debe_Calcular_Balance_Correctamente()
        {
            var incomes = new List<Income>
            {
                new Income { Amount = 1000 }
            };

            var expenses = new List<Expense>
            {
                new Expense { Amount = 300 }
            };

            var totalIncome = incomes.Sum(i => i.Amount);
            var totalExpense = expenses.Sum(e => e.Amount);

            var balance = totalIncome - totalExpense;

            Assert.Equal(700, balance);
        }
    }
}

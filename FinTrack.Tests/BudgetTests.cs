using FinTrack.Core.Entities;
using Xunit;

namespace FinTrack.Tests
{
    public class BudgetTests
    {
        [Fact]
        public void Debe_Crear_Gasto_Correctamente()
        {
            var budget = new Budget
            {
                Amount = 2000
            };

            Assert.Equal(2000, budget.Amount);
        }
    }
}

using FinTrack.Core.Entities;
using Xunit;

namespace FinTrack.Tests
{
    public class ExpenseTests
    {
        [Fact]
        public void Debe_Crear_Income_Con_Datos_Validos()

        {
            var expense = new Expense
            {
                Amount = 500,
                Description = "Comida"
            };

            Assert.Equal(500, expense.Amount);
            Assert.Equal("Comida", expense.Description);
        }
    }
}

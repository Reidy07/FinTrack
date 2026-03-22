namespace FinTrack.Core.DTOs.Category
{
    public class CategoryDetailDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        // Totales calculados
        public decimal TotalAmount { get; set; }
        public decimal CurrentMonthAmount { get; set; }
        public decimal PreviousMonthAmount { get; set; }

        // Historial agrupado por mes (Ej: "Marzo 2026": $500.00)
        public Dictionary<string, decimal> MonthlyTotals { get; set; } = new();

        // Lista de todos los gastos/ingresos de esta categoría
        public List<CategoryTransactionDto> Transactions { get; set; } = new();
    }

    public class CategoryTransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string TransactionType { get; set; } = string.Empty; // "Ingreso" o "Gasto"
    }
}

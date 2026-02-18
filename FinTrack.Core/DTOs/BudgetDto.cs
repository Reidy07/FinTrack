namespace FinTrack.Core.DTOs
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Para mostrar en listas
    }
}

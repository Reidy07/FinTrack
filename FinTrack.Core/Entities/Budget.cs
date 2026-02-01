namespace FinTrack.Core.Entities
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

       
        // Puede ser por categoría o general
        public int? CategoryId { get; set; }  // Nullable = presupuesto general

        
        // Foreign Keys
        public string UserId { get; set; }

        
        // Navigation
        public User User { get; set; }
        public Category Category { get; set; }
    }
}

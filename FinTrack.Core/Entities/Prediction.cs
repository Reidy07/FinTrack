namespace FinTrack.Core.Entities
{
    public class Prediction
    {
        public int Id { get; set; }
        public DateTime PredictionDate { get; set; }  // Fecha para la que predice
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public decimal PredictedAmount { get; set; }
        public decimal Confidence { get; set; }  // 0-100% de confianza

       
        // Para qué categoría se predice (puede ser null para predicción general)
        public int? CategoryId { get; set; }

        
        // Foreign Keys
        public string UserId { get; set; }

        
        // Navigation
        public User User { get; set; }
        public Category Category { get; set; }
    }
}

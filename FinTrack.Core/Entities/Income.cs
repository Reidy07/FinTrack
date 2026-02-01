namespace FinTrack.Core.Entities
{
    public class Income
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        // Foreign Keys
        public string UserId { get; set; }  // string por Identity
        public int CategoryId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Category Category { get; set; }
    }
}

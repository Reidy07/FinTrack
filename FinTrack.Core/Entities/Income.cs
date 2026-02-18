using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Core.Entities
{
    public class Income
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        // Foreign Keys
        public string UserId { get; set; } = string.Empty;  // string por Identity
        public int CategoryId { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
    }
}

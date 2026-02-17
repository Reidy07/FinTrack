using FinTrack.Core.Enum;

namespace FinTrack.Core.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; } // Hex: #FFFFFF
        public CategoryType Type { get; set; }
    }
}

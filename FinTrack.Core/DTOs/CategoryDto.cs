using FinTrack.Core.Enum;

namespace FinTrack.Core.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty; // Hex: #FFFFFF
        public CategoryType Type { get; set; }
    }
}

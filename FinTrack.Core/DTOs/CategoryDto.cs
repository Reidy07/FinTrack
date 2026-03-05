using FinTrack.Core.Enum;

namespace FinTrack.Core.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; } // Hex: #FFFFFF
        public CategoryType Type { get; set; }
    }
}

namespace FinTrack.Core.DTOs
{
    public class CategoryComparisonDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Data { get; set; } = new();
    }
}

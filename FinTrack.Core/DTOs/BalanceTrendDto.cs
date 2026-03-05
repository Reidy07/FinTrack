namespace FinTrack.Core.DTOs
{
    public class BalanceTrendDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Balances { get; set; } = new();
    }
}

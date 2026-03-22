namespace FinTrack.Core.DTOs.Reports
{
    public class BalanceTrendDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Balances { get; set; } = new();
    }
}

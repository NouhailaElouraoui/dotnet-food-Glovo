using Nouhaila.netProjet.Models;

namespace Nouhaila.netProjet.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        
        // New Statistics
        public int OrdersToday { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int LoyaltyPointsDistributed { get; set; }

        // Chart Data (JSON)
        public string DailyRevenueJson { get; set; } = "[]";
        public string DailyOrdersJson { get; set; } = "[]";

        public List<Order> LatestOrders { get; set; } = new();
    }
}

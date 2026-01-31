using Microsoft.AspNetCore.SignalR;

namespace Nouhaila.netProjet.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinOrderGroup(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }

        public async Task LeaveOrderGroup(int orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }
    }
}

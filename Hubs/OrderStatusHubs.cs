using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Bookings_Hotel.Hubs
{
    public class OrderStatusHub : Hub
    {
        // Phương thức này sẽ được gọi khi trạng thái đơn hàng thay đổi
        public async Task SendOrderStatusUpdate(int orderId, string newStatus)
        {
            await Clients.All.SendAsync("ReceiveOrderStatusUpdate", orderId, newStatus);
        }
    }
}

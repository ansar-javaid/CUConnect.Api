using Microsoft.AspNetCore.SignalR;


namespace CUConnect.Logic.Notifications
{
    public class NotificationHub : Hub
    {
        public async Task SendOffersToUser(string message)
        {
            await Clients.All.SendAsync("Notification", message);
        }
    }
}

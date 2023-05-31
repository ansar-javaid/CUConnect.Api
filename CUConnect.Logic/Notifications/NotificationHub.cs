using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.SignalR;


namespace CUConnect.Logic.Notifications
{
    public class NotificationHub : Hub
    {
        public async Task SendOffersToUser(NotificationRES notification)
        {
            await Clients.All.SendAsync("Notification", notification);
        }
    }
}

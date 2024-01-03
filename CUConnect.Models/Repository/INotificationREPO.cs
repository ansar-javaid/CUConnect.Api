using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Models.Repository
{
    public interface INotificationREPO
    {
        public Task<IActionResult> GetExpoNotificationToken(ExpoTokenView expoTokenView);

        public Task SendExpoNotificationToAll(NotificationView notificationView);
    }
}

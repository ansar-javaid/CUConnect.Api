using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationREPO _notification;
        public NotificationController(INotificationREPO notification)
        {
            _notification = notification;
        }



        /// <summary>
        /// Saves the Expo token from client to database
        /// </summary>
        /// <param name="expoTokenView"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveExpoToken")]
        public async Task<ActionResult> GetExpoNotificationToken([FromBody] ExpoTokenView expoTokenView)
        {
            return Ok(await _notification.GetExpoNotificationToken(expoTokenView));
        }

        /// <summary>
        /// Send the Notification to all available tokens to coresponding devices
        /// </summary>
        /// <param name="notificationView"></param>
        /// <returns></returns>
        [HttpPost, Route("SendNotificationAll")]
        public async Task<ActionResult> SendExpoNotificationToAll([FromBody] NotificationView notificationView)
        {
            await _notification.SendExpoNotificationToAll(notificationView);
            return Ok();
        }
    }
}

using CUConnect.Logic.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CUConnect.Api.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private IHubContext<NotificationHub> _hubContext;
        public NotificationsController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("sendMessage")]
        public IActionResult SendNotification(string userEmail, string notification)
        {
            // Get the connection IDs associated with the user's email
            var connections = UserConnectionManager.Instance.GetConnections(userEmail);

            if (connections.Count > 0)
            {
                // Send the notification to each connection
                foreach (var connectionId in connections)
                {
                    _hubContext.Clients.Client(connectionId).SendAsync("notification", notification);
                }

                return Ok("Notification sent successfully.");
            }

            return NotFound("No connections found for the user's email.");
        }

    }
}

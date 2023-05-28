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
        public async Task<IActionResult> Send(string message)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("Notification", message);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}

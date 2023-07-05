using CUConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CUConnect.Logic.Notifications
{
    [Authorize(Roles = nameof(Roles.User))]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string userEmail = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            UserConnectionManager.Instance.RemoveConnection(userEmail, connectionId);

            UserConnectionManager.Instance.AddConnection(userEmail, connectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, "Users");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            string userEmail = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            UserConnectionManager.Instance.RemoveConnection(userEmail, connectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Users");

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUsers(string userEmail, string notification)
        {
            // Retrieve the target email addresses from the backend
            List<string> targetEmails = GetTargetEmails(userEmail); // Implement this method to get the allowed email addresses

            if (targetEmails.Contains(userEmail))
            {
                // Send the notification to all connections associated with the user's email
                List<string> connections = UserConnectionManager.Instance.GetConnections(userEmail);
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("notification", notification);
                }
            }
        }

        private List<string> GetTargetEmails(string userEmail)
        {
            // Implement your logic to retrieve the allowed email addresses from the backend
            // This can be a database query, API call, or any other data retrieval method

            // Example: Hardcoding allowed email addresses for demonstration purposes
            List<string> allowedEmails = new List<string>()
            {
                "user@example.com",
                "user2@example.com",
                "Ansar@example.com"
            };

            return allowedEmails;
        }
    }
}

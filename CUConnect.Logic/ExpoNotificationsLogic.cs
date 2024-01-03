using CUConnect.Database.Entities;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CUConnect.Logic
{
    public class ExpoNotificationsLogic : ControllerBase, INotificationREPO
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _expoApiUrl = "https://exp.host/--/api/v2/push/send";
        // Db COntext
        private readonly CUConnectDBContext _dbContext;
        public ExpoNotificationsLogic(CUConnectDBContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }


        public async Task<IActionResult> GetExpoNotificationToken(ExpoTokenView expoTokenView)
        {
            var tokenResult = _dbContext.AspNetUsers.Where(x => x.Email == expoTokenView.Email).FirstOrDefault();
            if (tokenResult != null)
            {
                tokenResult.ExpoToken = expoTokenView.Token;
                _dbContext.Attach(tokenResult).State = EntityState.Modified;
                try
                {
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        public async Task SendExpoNotificationToAll(NotificationView notificationView)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var expoTokens = await _dbContext.AspNetUsers
                                                 .Where(x => x.ExpoToken != null)
                                                 .Select(x => x.ExpoToken)
                                                 .ToListAsync();

                var notificationTasks = new List<Task<HttpResponseMessage>>();

                foreach (var expoToken in expoTokens)
                {
                    var payload = new
                    {
                        to = expoToken,
                        title = notificationView.Title,
                        body = notificationView.Description,
                    };

                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(_expoApiUrl),
                        Content = content,
                    };

                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                    request.Headers.Add("Host", "exp.host");

                    // Add the Content-Type header to the request
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    notificationTasks.Add(client.SendAsync(request));

                    // Introduce a small delay to stay within rate limits
                    await Task.Delay(100); // 100 milliseconds
                }

                await Task.WhenAll(notificationTasks);

                var successfulResponses = notificationTasks.Count(response => response.Result.IsSuccessStatusCode);

                // Handle successful responses or log information
                Console.WriteLine($"{successfulResponses} notifications sent successfully");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}

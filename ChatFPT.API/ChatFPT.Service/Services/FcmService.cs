

using ChatFPT.Core.Utils;
using ChatFPT.Service.Interfaces;
using System.Text;
using System.Text.Json;

namespace ChatFPT.Service.Services
{
    public class FcmService : IFcmService
    {       
            private static readonly string FcmUrl = "https://fcm.googleapis.com/v1/projects/pushnoti-64c77/messages:send";
            private FirebaseAuthHelper _firebaseAuthHelper;
            public FcmService(FirebaseAuthHelper firebaseAuthHelper)
            {
                _firebaseAuthHelper = firebaseAuthHelper;
            }
            public async Task SendNotificationAsync(string deviceToken, string title, string body)
            {
                var accessToken = await _firebaseAuthHelper.GetAccessTokenAsync();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    var payload = new
                    {
                        message = new
                        {
                            token = deviceToken,
                            notification = new
                            {
                                title = title,
                                body = body,
                            }
                        }
                    };

                    string jsonPayload = JsonSerializer.Serialize(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(FcmUrl, content);
                    string result = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }


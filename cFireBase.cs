using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace CoinAPI
{
    public class cFirebase
    {
        public static string? token;
        public static async Task<string> fnGetFirebaseToken()
        {   
            GoogleCredential credential;
            try
            {
                using (var stream = new FileStream("FirebaseConfig.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                }

                var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                return token;
            }
            catch (Exception pExc)
            {
                Exception exc = pExc;
                throw exc;
            }
            
        }

        public static async Task fnSendNotification(string token, string title, string body)
        {
            string accessToken = token; //"YOUR_ACCESS_TOKEN"; // Erişim tokenını alın
            string fcmUrl = "https://fcm.googleapis.com/v1/projects/notification-app-53368/messages:send";

            var payload = new
            {
                message = new
                {
                    topic = "test_topic",
                    data = new
                    {
                        title = title,
                        body = body
                    }
                }
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.PostAsync(fcmUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Yanıt: {response.StatusCode}");
                Console.WriteLine($"Detay: {responseContent}");
            }
        

            //using (var httpClient  = new HttpClient())
            //{
            //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            //    var message = new
            //    {
            //        message = new
            //        {
            //            //token = token,
            //            title = title,
            //            body = body
            //        }
            //    };
            //    var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);
            //    var content = new StringContent(jsonMessage, System.Text.Encoding.UTF8, "application/json");
            //    Task<HttpResponseMessage> response = httpClient.PostAsync("https://fcm.googleapis.com/v1/projects/notification-app-53368/messages:send", content);
            //    HttpResponseMessage result = response.Result;
            //}            
        }
    }
}

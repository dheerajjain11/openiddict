using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClientApp
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        public static async Task MainAsync(string[] args)
        {
            var client = new HttpClient();

            const string email = "dheeraj.jain@gmail.com", password = "Pass@word1";

            await CreateAccountAsync(client, email, password);

            var token = await GetTokenAsync(client, email, password);
            Console.WriteLine("Access token: {0}", token);
            Console.WriteLine();
            // var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc1RllQMVNUWkZXLUJERlpQUVIzVFlYVTE2WVdWMlhLTU5ZWkw5QlciLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiI0YTNiMzM3NC0xOWIyLTQzMzYtYjk2Ny1jYWEwZjliMDQ3MzEiLCJuYW1lIjoiZGhlZXJhai5qYWluQGdtYWlsLmNvbSIsInJvbGUiOiJVc2VyIiwidG9rZW5fdXNhZ2UiOiJhY2Nlc3NfdG9rZW4iLCJqdGkiOiIxYWUzZTY2MC0xYjFmLTRiMGMtOWI3NS01Y2Q5YzY2OTU5YzUiLCJhdWQiOlsicmVzb3VyY2Utc2VydmVyIiwiaHR0cDovL2xvY2FsaG9zdDo2MzM2NC8iXSwibmJmIjoxNTY2MDI0NDE0LCJleHAiOjE1NjYwMjgwMTQsImlhdCI6MTU2NjAyNDQxNCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwLyJ9.ISBNPdnHPYsCxHoZVbJchW-G5hjJ6ARf-cl-dABll9OtGXei07fLyJgsomx-WcKT1er5rkwGUXYIj7JewWSFQAbv5PtRJ55EvdLNGJm089_usFPOceih944-E5SvAgvMDIg_KFjobgx4Z7JlFAIiDwLUyO_K8ydXSc8S95emwnhNOk61XpcbvuJmgMwnUeHpLmDHO1HZxtdQz5Ey5rX_W14NVE_pEV9iUAqmP0WNFyOJ7SIPTFDBfclHxNL-os2S9BwzqJcboxQS4WxRpECPrhWzW-CSfjU-Q_TY0k6DrhdEFP9MQTaXGgAZDgiFQfy0U-ANFzlTmG0VNhbbKNAMfA";
            //var resource = await GetResourceAsync(client, token);
            //var resource = await GetResourceAdminAsync(client, token);
            var resource = await GetSeparateResourceAsync(client, token);
            Console.WriteLine("API response: {0}", resource);
            Console.ReadLine();
        }

        public static async Task CreateAccountAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/Account/Register")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { email, password }), Encoding.UTF8, "application/json")
            };

            // Ignore 409 responses, as they indicate that the account already exists.
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        public static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = email,
                ["password"] = password
            });

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload["error"] != null)
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return (string) payload["access_token"];
        }

        public static async Task<string> GetResourceAsync(HttpClient client, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/message");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetSeparateResourceAsync(HttpClient client, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:63364/api/values/5");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetResourceAdminAsync(HttpClient client, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/api/messageadmin");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}

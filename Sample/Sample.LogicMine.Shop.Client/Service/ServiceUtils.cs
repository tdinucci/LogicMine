using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Shop.Client.Service
{
    public static class ServiceUtils
    {
        public static async Task<JObject> SendAsync(HttpClient httpClient, string serviceUrl, string securityToken,
            string requestDescription, string json)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(securityToken));
            if (string.IsNullOrWhiteSpace(requestDescription))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(requestDescription));
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(json));

            using (var request = new HttpRequestMessage(HttpMethod.Post, serviceUrl))
            {
                if (!string.IsNullOrWhiteSpace(securityToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", securityToken);

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    JObject responseJobj;
                    try
                    {
                        responseJobj = JObject.Parse(responseContent);
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException(
                            $"Invalid response from server while processing '{requestDescription}': {responseContent}");
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        if (!responseJobj.TryGetValue("error", out var error))
                        {
                            throw new InvalidOperationException(
                                $"Error processing '{requestDescription}': {responseContent}");
                        }

                        throw new InvalidOperationException(
                            $"Error processing '{requestDescription}': {error.Value<string>()}");
                    }

                    return responseJobj;
                }
            }
        }
    }
}
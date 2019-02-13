using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Shop.Client.Service
{
    public class LoginManager
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUrl;

        public LoginManager(HttpClient httpClient, string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serviceUrl = serviceUrl;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            var request = new JObject
            {
                {"requestType", "login"},
                {"username", username},
                {"password", password}
            };

            var response = await ServiceUtils.SendAsync(_httpClient, _serviceUrl, null, "login", request.ToString())
                .ConfigureAwait(false);

            if (!response.TryGetValue("accessToken", out var accessToken))
                throw new InvalidOperationException("Did not receive an access token after login");

            return accessToken.Value<string>();
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Sample.LogicMine.Shop.Client.Service
{
    public class SalesSummaryManager
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUrl;
        private readonly string _securityToken;

        public SalesSummaryManager(HttpClient httpClient, string serviceUrl, string securityToken)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));
            if (string.IsNullOrWhiteSpace(securityToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(securityToken));

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serviceUrl = serviceUrl;
            _securityToken = securityToken;
        }

        public async Task<SalesSummary> GetAsync()
        {
            var opDesc = "get sales summary";
            var request = new JObject {{"requestType", "salesSummary"}};

            var response = await ServiceUtils
                .SendAsync(_httpClient, _serviceUrl, _securityToken, opDesc, request.ToString())
                .ConfigureAwait(false);

            return response.ToObject<SalesSummary>();
        }
    }
}
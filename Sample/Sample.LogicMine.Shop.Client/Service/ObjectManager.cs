using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Sample.LogicMine.Shop.Client.Service
{
    public class ObjectManager<T> where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUrl;
        private readonly string _securityToken;

        public ObjectManager(HttpClient httpClient, string serviceUrl, string securityToken)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));
            if (string.IsNullOrWhiteSpace(securityToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(securityToken));

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serviceUrl = serviceUrl;
            _securityToken = securityToken;
        }

        public async Task<int> CreateAsync(T obj)
        {
            var request = new JObject
            {
                {"requestType", "createObject"},
                {"type", typeof(T).Name},
                {"object", JToken.FromObject(obj)}
            };

            var response = await SendRequestAsync($"create {typeof(T).Name}", request).ConfigureAwait(false);

            if (!response.TryGetValue("objectId", out var objectId))
            {
                throw new InvalidOperationException(
                    $"Saving '{request.Type}' appeared successful however did not receive an id");
            }

            return objectId.Value<int>();
        }

        public async Task<T> GetAsync(int id)
        {
            var request = new JObject
            {
                {"requestType", "getObject"},
                {"type", typeof(T).Name},
                {"id", id}
            };

            var response = await SendRequestAsync($"get {typeof(T).Name}", request).ConfigureAwait(false);

            if (!response.TryGetValue("object", out var obj))
                throw new InvalidOperationException($"Getting '{request.Type}' failed");

            return obj.ToObject<T>();
        }

        public async Task<T[]> GetCollectionAsync(string filter = null, int? max = null, int? page = null)
        {
            var request = new JObject
            {
                {"requestType", "getCollection"},
                {"type", typeof(T).Name},
                {"filter", filter},
                {"max", max},
                {"page", page}
            };

            var response = await SendRequestAsync($"get collection of {typeof(T).Name}", request).ConfigureAwait(false);

            if (!response.TryGetValue("objects", out var objects))
                throw new InvalidOperationException($"Getting collection of '{request.Type}' failed");

            return objects.ToObject<T[]>();
        }

        private Task<JObject> SendRequestAsync(string description, JObject request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var json = JsonConvert.SerializeObject(request, settings);

            return ServiceUtils.SendAsync(_httpClient, _serviceUrl, _securityToken, description, json);
        }
    }
}
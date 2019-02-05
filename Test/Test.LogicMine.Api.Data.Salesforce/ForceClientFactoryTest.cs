using System.Threading.Tasks;
using LogicMine.Api.Data.Salesforce;
using Xunit;

namespace Test.LogicMine.Api.Data.Salesforce
{
    public class ForceClientFactoryTest
    {
        private const string ClientId = "XXX";
        private const string ClientSecret = "XXX";
        private const string Username = "XXX@XXX.com";
        private const string Password = "XXX";
        private const bool IsProduction = false;

        [Fact]
        public async Task ConstructOne()
        {
            var client = await ForceClientFactory.CreateAsync(ClientId, ClientSecret, Username, Password, IsProduction)
                .ConfigureAwait(false);

            Assert.NotNull(client);
        }

        [Fact]
        public async Task ContructMultipleSame()
        {
            var client = await ForceClientFactory.CreateAsync(ClientId, ClientSecret, Username, Password, IsProduction)
                .ConfigureAwait(false);

            var client2 = await ForceClientFactory.CreateAsync(ClientId, ClientSecret, Username, Password, IsProduction)
                .ConfigureAwait(false);

            var clientId = "ci_cat_id";
            var clientSecret = "ci_cat_secret";

            var client3 = await ForceClientFactory.CreateAsync(clientId, clientSecret, Username, Password, IsProduction)
                .ConfigureAwait(false);

            Assert.NotNull(client);
            Assert.NotNull(client2);
            Assert.NotNull(client3);
        }
    }
}
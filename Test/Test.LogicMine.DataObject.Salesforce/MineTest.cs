using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.Salesforce;
using Salesforce.Force;
using Xunit;

namespace Test.LogicMine.DataObject.Salesforce
{
    public class MineTest
    {
        private const string SfClientId =
            "3MVG9ZPHiJTk7yFyo2kgZvLTvpjobQskYGDyhEnON21Vz1BfOAbXSOBrvM395NJsBVhCgIck6IoESDreYY6Ah";

        private const string SfClientSecret = "8204173310639812200";
        private const string SfUsername = "";
        private const string SfPassword = "";
        private const string SfAuthEndpoint = "https://test.salesforce.com/services/oauth2/token";

        private const string AccessTokenKey = "AccessToken";
        private const string ValidAccessToken = "987654";

        private IDataObjectDescriptorRegistry GetDescriptorRegistry()
        {
            return new DataObjectDescriptorRegistry()
                .Register(new MyContactObjectDescriptor());
        }

        private IMine CreateMine<T>(ITraceExporter traceExporter, IDataObjectDescriptorRegistry descriptorRegistry)
            where T : new()
        {
            var sfConfig =
                new SalesforceCredentials(SfClientId, SfClientSecret, SfUsername, SfPassword, SfAuthEndpoint);
            var descriptor = descriptorRegistry.GetDescriptor<T, SalesforceObjectDescriptor<T>>();

            var objectStore = new SalesforceObjectStore<T>(sfConfig, descriptor);

            return new Mine()
                .AddShaft(new Shaft<GetObjectRequest<T, string>, GetObjectResponse<T>>(traceExporter,
                    new GetObjectTerminal<T, string>(objectStore),
                    new SecurityStation()))
                .AddShaft(new Shaft<ReverseStringRequest, RevereStringResponse>(traceExporter,
                    new ReverseStringTerminal(),
                    new SecurityStation()));
        }

        [Fact]
        public async Task GetObject()
        {
            const string contactId = "0030Q000006RJUc";

            var traceExporter = new MyTraceExporter();
            var mine = CreateMine<MyContact>(traceExporter, GetDescriptorRegistry());

            var request = new GetObjectRequest<MyContact, string>(contactId);
            request.Options.Add(AccessTokenKey, ValidAccessToken);

            var response = await mine
                .SendAsync<GetObjectRequest<MyContact, string>, GetObjectResponse<MyContact>>(request)
                .ConfigureAwait(false);

            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
            Assert.Null(response.Error);
            Assert.StartsWith(contactId, response.Object.Id);
            Assert.Equal("United Kingdom", response.Object.Country);
        }

        [Fact]
        public async Task BadAccessToken()
        {
            var traceExporter = new MyTraceExporter();
            var mine = CreateMine<MyContact>(traceExporter, GetDescriptorRegistry());

            var request = new GetObjectRequest<MyContact, string>("0030Q000006RJUc");
            request.Options.Add(AccessTokenKey, "xxxx");

            var response = await mine
                .SendAsync<GetObjectRequest<MyContact, string>, GetObjectResponse<MyContact>>(request)
                .ConfigureAwait(false);

            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
            Assert.NotNull(response.Error);
            Assert.Null(response.Object);
            Assert.Equal("Invalid access token", response.Error);
        }

        [Fact]
        public async Task ReverseString()
        {
            var traceExporter = new MyTraceExporter();
            var mine = CreateMine<MyContact>(traceExporter, GetDescriptorRegistry());

            var forward = "hello there";
            var expectedReversed = "ereht olleh";

            var request = new ReverseStringRequest(forward);
            request.Options.Add(AccessTokenKey, ValidAccessToken);

            var response = await mine.SendAsync<ReverseStringRequest, RevereStringResponse>(request)
                .ConfigureAwait(false);

            Assert.Equal(expectedReversed, response.Reversed);
        }

        private class MyTraceExporter : ITraceExporter
        {
            public string Trace { get; private set; }
            public string Error { get; private set; }

            public void Export(IBasket basket)
            {
                var sb = new StringBuilder();
                foreach (var visit in basket.Visits)
                    sb.AppendLine($"{visit.Description} {visit.Direction}");

                Trace = sb.ToString();
            }

            public void ExportError(Exception exception)
            {
                Error = exception.Message;
            }

            public void ExportError(string error)
            {
                Error = error;
            }
        }

        private class MyContact
        {
            public string Id { get; set; }
            public string Forename { get; set; }
            public string Surname { get; set; }
            public string Country { get; set; }
            public string PostalCode { get; set; }
        }

        private class MyContactObjectDescriptor : SalesforceObjectDescriptor<MyContact>
        {
            public MyContactObjectDescriptor() : base("Contact", "Id")
            {
            }

            public override string GetMappedColumnName(string propertyName)
            {
                switch (propertyName)
                {
                    case nameof(MyContact.Forename):
                        return "FirstName";
                    case nameof(MyContact.Surname):
                        return "LastName";
                    case nameof(MyContact.Country):
                        return "MailingCountry";
                    case nameof(MyContact.PostalCode):
                        return "MailingPostalCode";
                    default:
                        return base.GetMappedColumnName(propertyName);
                }
            }
        }

        private class ReverseStringRequest : Request
        {
            public string ToReverse { get; }

            public ReverseStringRequest(string toReverse)
            {
                ToReverse = toReverse;
            }
        }

        private class RevereStringResponse : Response
        {
            public string Reversed { get; }

            public RevereStringResponse(IRequest request) : base(request)
            {
            }

            public RevereStringResponse(IRequest request, string reversed) : this(request)
            {
                Reversed = reversed;
            }
        }

        private class ReverseStringTerminal : Terminal<ReverseStringRequest, RevereStringResponse>
        {
            public override Task AddResponseAsync(IBasket<ReverseStringRequest, RevereStringResponse> basket)
            {
                var reversed = new string(basket.Request.ToReverse.Reverse().ToArray());
                basket.Response = new RevereStringResponse(basket.Request, reversed);

                return Task.CompletedTask;
            }
        }

        private class SecurityStation : Station<IRequest, IResponse>
        {
            public override Task DescendToAsync(IBasket<IRequest, IResponse> basket)
            {
                if (basket.Request.Options.TryGetValue(AccessTokenKey, out var accessToken))
                {
                    if ((string) accessToken == ValidAccessToken)
                        return Task.CompletedTask;
                }

                throw new InvalidOperationException("Invalid access token");
            }

            public override Task AscendFromAsync(IBasket<IRequest, IResponse> basket)
            {
                return Task.CompletedTask;
            }
        }
    }
}
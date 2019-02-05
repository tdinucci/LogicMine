using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Data.Salesforce;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Filter;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using Newtonsoft.Json.Linq;
using Salesforce.Force;
using Xunit;

namespace Test.LogicMine.Api.Data.Salesforce
{
    public class SalesforceLayerTest
    {
        private const string ClientId = "XXX";
        private const string ClientSecret = "XXX";
        private const string Username = "XXX";
        private const string Password = "XXX";
        private const bool IsProduction = false;

        private readonly IForceClient _forceClient;

        public SalesforceLayerTest()
        {
            _forceClient = ForceClientFactory.CreateAsync(ClientId, ClientSecret, Username, Password, IsProduction)
                .GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetCollection()
        {
            var layer = new SalesforceLayer<MyContact>(_forceClient, new MyContactObjectDescriptor());

            var getBasket = new GetCollectionBasket<MyContact>(new GetCollectionRequest<MyContact>(10, 20));
            await layer.AddResultAsync(getBasket, new Visit("GetCol", VisitDirections.Down)).ConfigureAwait(false);

            Assert.True(getBasket.AscentPayload.Length == 10);
        }

        [Fact]
        public async Task Post()
        {
            var layer = new SalesforceLayer<MyContact>(_forceClient, new MyContactObjectDescriptor());

            var contact = new MyContact
            {
                Forename = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = "Glasgow",
                Country = "United Kingdom",
                PostalCode = "G12AB",
                CanEmail = true
            };

            var postBasket = new PostBasket<MyContact, string>(contact);
            await layer.AddResultAsync(postBasket, new Visit("Post", VisitDirections.Down)).ConfigureAwait(false);

            var id = postBasket.AscentPayload;
            Assert.False(string.IsNullOrWhiteSpace(id));

            var getBasket = new GetBasket<string, MyContact>(id);
            await layer.AddResultAsync(getBasket, new Visit("Get", VisitDirections.Down)).ConfigureAwait(false);

            var readContact = getBasket.AscentPayload;
            Assert.Equal(id, readContact.Id);
            Assert.Equal(contact.Forename, readContact.Forename);
            Assert.Equal(contact.Surname, readContact.Surname);
            Assert.Equal(contact.Street, readContact.Street);
            Assert.Equal(contact.City, readContact.City);
            Assert.Equal(contact.Country, readContact.Country);
            Assert.Equal(contact.PostalCode, readContact.PostalCode);
            Assert.Equal(contact.CanMailshot, readContact.CanMailshot);
            Assert.Equal(contact.CanEmail, readContact.CanEmail);
            Assert.Equal(contact.CanPhone, readContact.CanPhone);
        }

        [Fact]
        public async Task Patch()
        {
            var layer = new SalesforceLayer<MyContact>(_forceClient, new MyContactObjectDescriptor());

            var contact = new MyContact
            {
                Forename = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = "Glasgow",
                Country = "United Kingdom",
                PostalCode = "G12AB",
                CanEmail = true
            };

            var postBasket = new PostBasket<MyContact, string>(contact);
            await layer.AddResultAsync(postBasket, new Visit("Post", VisitDirections.Down)).ConfigureAwait(false);

            var id = postBasket.AscentPayload;
            Assert.False(string.IsNullOrWhiteSpace(id));

            var patchRequest = new PatchRequest<string, MyContact>(
                new Delta<string, MyContact>(id, new Dictionary<string, object>
                {
                    {nameof(MyContact.Forename), "Jimmy"},
                    {nameof(MyContact.Surname), "Riddle"}
                }));
            var patchBasket = new PatchBasket<string, MyContact, int>(patchRequest);
            await layer.AddResultAsync(patchBasket, new Visit("Patch", VisitDirections.Down)).ConfigureAwait(false);
            
            Assert.Equal(1, patchBasket.AscentPayload);
            
            var getBasket = new GetBasket<string, MyContact>(id);
            await layer.AddResultAsync(getBasket, new Visit("Get", VisitDirections.Down)).ConfigureAwait(false);
            
            var readContact = getBasket.AscentPayload;
            Assert.Equal(id, readContact.Id);
            Assert.Equal("Jimmy", readContact.Forename);
            Assert.Equal("Riddle", readContact.Surname);
            Assert.Equal(contact.Street, readContact.Street);
            Assert.Equal(contact.City, readContact.City);
            Assert.Equal(contact.Country, readContact.Country);
            Assert.Equal(contact.PostalCode, readContact.PostalCode);
            Assert.Equal(contact.CanMailshot, readContact.CanMailshot);
            Assert.Equal(contact.CanEmail, readContact.CanEmail);
            Assert.Equal(contact.CanPhone, readContact.CanPhone);
        }

        [Fact]
        public async Task Delete()
        {
            var layer = new SalesforceLayer<MyContact>(_forceClient, new MyContactObjectDescriptor());

            var contact = new MyContact
            {
                Forename = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = "Glasgow",
                Country = "United Kingdom",
                PostalCode = "G12AB",
                CanEmail = true
            };

            var postBasket = new PostBasket<MyContact, string>(contact);
            await layer.AddResultAsync(postBasket, new Visit("Post", VisitDirections.Down)).ConfigureAwait(false);

            var id = postBasket.AscentPayload;
            Assert.False(string.IsNullOrWhiteSpace(id));

            var deleteBasket = new DeleteBasket<string, MyContact, int>(id);
            await layer.AddResultAsync(deleteBasket, new Visit("Delete", VisitDirections.Down)).ConfigureAwait(false);

            Assert.Equal(1, deleteBasket.AscentPayload);

            Exception exception = null;

            try
            {
                var getBasket = new GetBasket<string, MyContact>(id);
                await layer.AddResultAsync(getBasket, new Visit("Get", VisitDirections.Down)).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.Contains("Done: True, Count: 0", exception.Message);
        }

        [Fact]
        public async Task DeleteCollection()
        {
            var layer = new SalesforceLayer<MyContact>(_forceClient, new MyContactObjectDescriptor());

            var deleteBasket = new DeleteCollectionBasket<MyContact, int>(
                new DeleteCollectionRequest<MyContact>(new Filter<MyContact>(new[]
                {
                    new FilterTerm(nameof(MyContact.Forename), FilterOperators.Equal, "Tim"),
                })));

            Exception exception = null;
            try
            {
                await layer.AddResultAsync(deleteBasket, new Visit("DeleteC", VisitDirections.Down))
                    .ConfigureAwait(false);
            }
            catch (NotSupportedException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
        }

        private class MyContact
        {
            public string Id { get; set; }
            public string Forename { get; set; }
            public string Surname { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string PrimaryLanguage { get; set; }

            public string Street { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }

            public string Email { get; set; }
            public string HomePhone { get; set; }
            public string MobilePhone { get; set; }

            public bool CanMailshot { get; set; }
            public bool CanEmail { get; set; }
            public bool CanText { get; set; }
            public bool CanPhone { get; set; }

            public string BrandId { get; set; }
            public string DesiredEmploymentType { get; set; }
            public string Status { get; set; }
        }

        private class MyContactObjectDescriptor : SalesforceObjectDescriptor
        {
            public MyContactObjectDescriptor() : base("Contact")
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
                    case nameof(MyContact.DateOfBirth):
                        return "Birthdate";
                    case nameof(MyContact.PrimaryLanguage):
                        return "Language__c";
                    case nameof(MyContact.Street):
                        return "MailingStreet";
                    case nameof(MyContact.City):
                        return "MailingCity";
                    case nameof(MyContact.Province):
                        return "MailingState";
                    case nameof(MyContact.PostalCode):
                        return "MailingPostalCode";
                    case nameof(MyContact.Country):
                        return "MailingCountry";

                    case nameof(MyContact.CanMailshot):
                        return "Receive_Consultant_Mailshots__c";
                    case nameof(MyContact.CanEmail):
                        return "Can_Email__c";
                    case nameof(MyContact.CanPhone):
                        return "Can_Phone__c";
                    case nameof(MyContact.CanText):
                        return "Can_SMS__c";

                    case nameof(MyContact.BrandId):
                        return "Brand__c";
                    case nameof(MyContact.DesiredEmploymentType):
                        return "Candidate_Type__c";
                    case nameof(MyContact.Status):
                        return "Status__c";

                    default:
                        return base.GetMappedColumnName(propertyName);
                }
            }

            public override void PrepareForPost(JObject objToPost)
            {
                objToPost["Salutation"] = ".";
                base.PrepareForPost(objToPost);
            }
        }
    }
}
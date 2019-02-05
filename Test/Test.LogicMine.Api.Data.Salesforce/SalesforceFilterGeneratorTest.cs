using System;
using LogicMine.Api.Data.Salesforce;
using LogicMine.Api.Filter;
using Xunit;

namespace Test.LogicMine.Api.Data.Salesforce
{
    public class SalesforceFilterGeneratorTest
    {
        [Fact]
        public void Construct()
        {
            var forenameFilter = Guid.NewGuid().ToString();
            var cityFilter = Guid.NewGuid().ToString();
            var dobFrom = new DateTime(1954, 2, 4);
            var dobTo = new DateTime(2001, 2, 4);
            var canEmailFilter = true;
            var canPhoneFilter = false;

            var filter = new Filter<MyContact>(new[]
            {
                new FilterTerm(nameof(MyContact.Forename), FilterOperators.Equal, forenameFilter),
                new FilterTerm(nameof(MyContact.City), FilterOperators.NotEqual, cityFilter),
                new RangeFilterTerm(nameof(MyContact.DateOfBirth), dobFrom, dobTo),
                new InFilterTerm(nameof(MyContact.Country), new[] {"UK", "France", "Spain"}),
                new FilterTerm(nameof(MyContact.CanEmail), FilterOperators.Equal, canEmailFilter),
                new FilterTerm(nameof(MyContact.CanPhone), FilterOperators.Equal, canPhoneFilter),
            });

            var descriptor = new MyContactObjectDescriptor();
            var gen = new SalesforceFilterGenerator(filter, descriptor.GetMappedColumnName);

            var whereClause = gen.Generate();

            var expectedWhere =
                $"WHERE FirstName = '{forenameFilter}' AND MailingCity != '{cityFilter}' " +
                $"AND Birthdate >= 1954-02-04T00:00:00Z AND Birthdate <= 2001-02-04T00:00:00Z " +
                $"AND MailingCountry IN ('UK','France','Spain') AND Can_Email__c = True AND Can_Phone__c = False";

            Assert.Equal(expectedWhere, whereClause);
        }

        private class ContactObjectDescriptor : SalesforceObjectDescriptor
        {
            public ContactObjectDescriptor() : base("Contact", new[] {"Id"})
            {
            }
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

            public decimal CurrentSalary { get; set; }
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
        }
    }
}
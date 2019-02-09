using System;
using LogicMine.DataObject.Salesforce;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Web.Mine.MyContact
{
    public class MyContactObjectDescriptor : SalesforceObjectDescriptor<MyContact>
    {
        public MyContactObjectDescriptor() : base("Contact", "Id")
        {
        }

        public override string GetMappedColumnName(string propertyName)
        {
            if (IsPropertyNameMatch(propertyName, nameof(MyContact.Forename)))
                return "FirstName";

            if (IsPropertyNameMatch(propertyName, nameof(MyContact.Surname)))
                return "LastName";

            if (IsPropertyNameMatch(propertyName, nameof(MyContact.Country)))
                return "MailingCountry";

            if (IsPropertyNameMatch(propertyName, nameof(MyContact.PostalCode)))
                return "MailingPostalCode";

            return base.GetMappedColumnName(propertyName);
        }

        public override void PrepareForPost(JObject objToPost)
        {
            objToPost["Salutation"] = ".";
            base.PrepareForPost(objToPost);
        }
    }
}
using LogicMine.DataObject.Salesforce;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Web.Mine.Manager
{
    public class ManagerObjectDescriptor : SalesforceObjectDescriptor<Manager>
    {
        public ManagerObjectDescriptor() : base("Contact", "Id")
        {
        }

        public override string GetMappedColumnName(string propertyName)
        {
            if (IsPropertyNameMatch(propertyName, nameof(Manager.Forename)))
                return "FirstName";

            if (IsPropertyNameMatch(propertyName, nameof(Manager.Surname)))
                return "LastName";

            if (IsPropertyNameMatch(propertyName, nameof(Manager.Country)))
                return "MailingCountry";

            if (IsPropertyNameMatch(propertyName, nameof(Manager.PostalCode)))
                return "MailingPostalCode";

            return base.GetMappedColumnName(propertyName);
        }
    }
}
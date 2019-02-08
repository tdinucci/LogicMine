using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.MyContact
{
    public class MyContactObjectDescriptor : SalesforceObjectDescriptor<MyContact>
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
}
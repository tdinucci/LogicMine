using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Candidate
{
    public class CandidateObjectDescriptor : SalesforceObjectDescriptor<Candidate>
    {
        public CandidateObjectDescriptor() : base("Contact", "Id")
        {
        }

        public override string GetMappedColumnName(string propertyName)
        {
            if (IsPropertyNameMatch(propertyName, nameof(Candidate.Forename)))
                return "FirstName";

            if (IsPropertyNameMatch(propertyName, nameof(Candidate.Surname)))
                return "LastName";

            if (IsPropertyNameMatch(propertyName, nameof(Candidate.Country)))
                return "MailingCountry";

            if (IsPropertyNameMatch(propertyName, nameof(Candidate.PostalCode)))
                return "MailingPostalCode";

            return base.GetMappedColumnName(propertyName);
        }
    }
}
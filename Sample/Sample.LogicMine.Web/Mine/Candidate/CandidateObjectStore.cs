using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Candidate
{
    public class CandidateObjectStore : SalesforceContactObjectStore<Candidate>
    {
        protected override string SalesforceContactType { get; } = "Candidate";

        public CandidateObjectStore(SalesforceCredentials credentials,
            SalesforceObjectDescriptor<Candidate> descriptor) : base(credentials, descriptor)
        {
        }
    }
}
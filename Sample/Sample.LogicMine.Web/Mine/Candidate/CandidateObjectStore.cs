using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Candidate
{
    public class CandidateObjectStore : SalesforceContactObjectStore<Candidate>
    {
        protected override string SalesforceContactType { get; } = "Candidate";

        public CandidateObjectStore(SalesforceConnectionConfig connectionConfig,
            SalesforceObjectDescriptor<Candidate> descriptor) : base(connectionConfig, descriptor)
        {
        }
    }
}
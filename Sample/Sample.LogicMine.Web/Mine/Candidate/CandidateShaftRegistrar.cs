using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Candidate
{
    public class CandidateShaftRegistrar : SampleDataObjectShaftRegistrar<Candidate>
    {
        public CandidateShaftRegistrar(SalesforceCredentials credentials,
            IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter) :
            base(credentials, descriptorRegistry, traceExporter)
        {
        }

        protected override IDataObjectStore<Candidate, string> GetDataObjectStore()
        {
            return new CandidateObjectStore(SalesforceCredentials, Descriptor);
        }
    }
}
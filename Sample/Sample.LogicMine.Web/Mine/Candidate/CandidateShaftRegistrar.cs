using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.Salesforce;

namespace Sample.LogicMine.Web.Mine.Candidate
{
    public class CandidateShaftRegistrar : SampleDataObjectShaftRegistrar<Candidate>
    {
        public CandidateShaftRegistrar(SalesforceConnectionConfig connectionConfig,
            IDataObjectDescriptorRegistry descriptorRegistry, ITraceExporter traceExporter) :
            base(connectionConfig, descriptorRegistry, traceExporter)
        {
        }

        protected override IDataObjectStore<Candidate, string> GetDataObjectStore()
        {
            return new CandidateObjectStore(SalesforceConnectionConfig, Descriptor);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine.DataObject.GetObject;
using Newtonsoft.Json.Linq;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceGetObjectTerminal<T> : Terminal<GetObjectRequest<T, string>, GetObjectResponse<T>>
        where T : new()
    {
        private readonly IForceClient _forceClient;
        private readonly SalesforceObjectDescriptor<T> _descriptor;

        public SalesforceGetObjectTerminal(IForceClient forceClient, SalesforceObjectDescriptor<T> descriptor)
        {
            _forceClient = forceClient ?? throw new ArgumentNullException(nameof(forceClient));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public override async Task AddResponseAsync(IBasket<GetObjectRequest<T, string>, GetObjectResponse<T>> basket)
        {
            var properties = _descriptor.GetReadableProperties();
            var selectList = properties
                .Select(p => _descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            var query =
                $"SELECT {selectList} FROM {_descriptor.SalesforceTypeName} WHERE Id = '{basket.Payload.Request.Id}'";

            var readObj = new T();
            var queryResult = await _forceClient.QueryAsync<JObject>(query).ConfigureAwait(false);
            if (queryResult.Done && queryResult.Records.Count == 1)
            {
                var sfObject = queryResult.Records[0];
                foreach (var property in properties)
                {
                    var mappedPropertyName = _descriptor.GetMappedColumnName(property.Name);
                    var propValue = sfObject[mappedPropertyName].ToObject(property.PropertyType);
                    var sfValue = _descriptor.ProjectColumnValue(propValue, property.PropertyType);

                    property.SetValue(readObj, sfValue);
                }

                basket.Payload.Response = new GetObjectResponse<T>(readObj);
            }
            else
            {
                basket.CurrentVisit.Log(
                    $"Query failed [{query}] - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");

                throw new InvalidOperationException(
                    $"Query failed - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");
            }
        }
    }
}
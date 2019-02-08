using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine.DataObject.GetCollection;
using Newtonsoft.Json.Linq;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceGetCollectionTerminal<T> : Terminal<GetCollectionRequest<T>, GetCollectionResponse<T>>
        where T : new()
    {
        private readonly IForceClient _forceClient;
        private readonly SalesforceObjectDescriptor<T> _descriptor;

        public SalesforceGetCollectionTerminal(IForceClient forceClient, SalesforceObjectDescriptor<T> descriptor)
        {
            _forceClient = forceClient ?? throw new ArgumentNullException(nameof(forceClient));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public override async Task AddResponseAsync(IBasket<GetCollectionRequest<T>, GetCollectionResponse<T>> basket)
        {
            var properties = _descriptor.GetReadableProperties();
            var selectList = properties
                .Select(p => _descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            var query = $"SELECT {selectList} FROM {_descriptor.SalesforceTypeName}";
            if (basket.Payload?.Request?.Filter?.Terms != null && basket.Payload.Request.Filter.Terms.Any())
            {
                var filterGenerator =
                    new SalesforceFilterGenerator(basket.Payload.Request.Filter, _descriptor.GetMappedColumnName);
                var where = filterGenerator.Generate();

                if (string.IsNullOrWhiteSpace(where))
                    throw new InvalidOperationException("Failed to generate WHERE clause");

                query += $" {where}";
            }

            if (basket.Payload?.Request?.Max.GetValueOrDefault(0) > 0)
            {
                query += $" LIMIT {basket.Payload.Request.Max}";
                if (basket.Payload.Request.Page.GetValueOrDefault(0) > 0)
                    query += $" OFFSET {basket.Payload.Request.Page * basket.Payload.Request.Max}";
            }

            var queryResult = await _forceClient.QueryAsync<JObject>(query).ConfigureAwait(false);
            if (queryResult.Done)
            {
                var result = new T[queryResult.Records.Count];
                for (var i = 0; i < result.Length; i++)
                {
                    var record = queryResult.Records[i];
                    var sfObject = new T();
                    result[i] = sfObject;

                    foreach (var property in properties)
                    {
                        var mappedPropertyName = _descriptor.GetMappedColumnName(property.Name);
                        var propValue = record[mappedPropertyName].ToObject(property.PropertyType);
                        var sfValue = _descriptor.ProjectColumnValue(propValue, property.PropertyType);

                        property.SetValue(sfObject, sfValue);
                    }
                }

                basket.Payload.Response = new GetCollectionResponse<T>(result);
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
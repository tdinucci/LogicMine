using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Salesforce.Common.Models.Json;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceClient
    {
        private readonly SalesforceCredentials _credentials;

        public SalesforceClient(SalesforceCredentials credentials)
        {
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        public async Task<QueryResult<JObject>> QueryAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));

            var client = await GetForceClientAsync().ConfigureAwait(false);
            return await client.QueryAsync<JObject>(query).ConfigureAwait(false);
        }

        public async Task<SuccessResponse> CreateAsync(string dataType, JObject record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (string.IsNullOrWhiteSpace(dataType))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dataType));

            var client = await GetForceClientAsync().ConfigureAwait(false);
            return await client.CreateAsync(dataType, record).ConfigureAwait(false);
        }

        public async Task<SuccessResponse> UpdateAsync(string dataType, string recordId, JObject record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (string.IsNullOrWhiteSpace(dataType))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dataType));
            if (string.IsNullOrWhiteSpace(recordId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(recordId));

            var client = await GetForceClientAsync().ConfigureAwait(false);
            return await client.UpdateAsync(dataType, recordId, record).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(string dataType, string recordId)
        {
            if (string.IsNullOrWhiteSpace(dataType))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dataType));
            if (string.IsNullOrWhiteSpace(recordId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(recordId));

            var client = await GetForceClientAsync().ConfigureAwait(false);
            return await client.DeleteAsync(dataType, recordId).ConfigureAwait(false);
        }

        private Task<IForceClient> GetForceClientAsync()
        {
            return ForceClientFactory.CreateAsync(_credentials);
        }
    }
}
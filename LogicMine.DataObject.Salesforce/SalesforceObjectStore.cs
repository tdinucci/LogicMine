using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LogicMine.DataObject.Filter;
using Newtonsoft.Json.Linq;
using Salesforce.Force;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceObjectStore<T> : IDataObjectStore<T, string>
        where T : new()
    {
        private readonly SalesforceConnectionConfig _connectionConfig;
        private readonly SalesforceObjectDescriptor<T> _descriptor;

        public SalesforceObjectStore(SalesforceConnectionConfig connectionConfig,
            SalesforceObjectDescriptor<T> descriptor)
        {
            _connectionConfig = connectionConfig ?? throw new ArgumentNullException(nameof(connectionConfig));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            if (_descriptor.DataType != typeof(T))
            {
                throw new ArgumentException(
                    $"The store is for '{typeof(T)}' but the supplied descriptor is for '{_descriptor.DataType}'");
            }
        }

        public async Task<T[]> GetCollectionAsync(IFilter<T> filter, int? max = null, int? page = null)
        {
            var forceClient = new SalesforceClient(_connectionConfig);
            var properties = _descriptor.GetReadableProperties();
            var selectList = properties
                .Select(p => _descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            var query = $"SELECT {selectList} FROM {_descriptor.SalesforceTypeName}";
            if (filter?.Terms != null && filter.Terms.Any())
            {
                var filterGenerator =
                    new SalesforceFilterGenerator(filter, _descriptor.GetMappedColumnName);
                var where = filterGenerator.Generate();

                if (string.IsNullOrWhiteSpace(where))
                    throw new InvalidOperationException("Failed to generate WHERE clause");

                query += $" {where}";
            }

            if (max.GetValueOrDefault(0) > 0)
            {
                query += $" LIMIT {max}";
                if (page.GetValueOrDefault(0) > 0)
                    query += $" OFFSET {page * max}";
            }

            var queryResult = await forceClient.QueryAsync(query).ConfigureAwait(false);
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

                return result;
            }

            throw new InvalidOperationException(
                $"Failed to retrieve collection of {typeof(T)}  - [{query}] - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");
        }

        public Task<T[]> GetCollectionAsync(int? max = null, int? page = null)
        {
            return GetCollectionAsync(null, max, page);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

            var forceClient = new SalesforceClient(_connectionConfig);
            var properties = _descriptor.GetReadableProperties();
            var selectList = properties
                .Select(p => _descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            var query =
                $"SELECT {selectList} FROM {_descriptor.SalesforceTypeName} WHERE Id = '{id}'";

            var readObj = new T();
            var queryResult = await forceClient.QueryAsync(query).ConfigureAwait(false);
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

                return readObj;
            }

            throw new InvalidOperationException(
                $"Failed to retrieve {typeof(T)}  - [{query}] - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");
        }

        public async Task<string> CreateAsync(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var forceClient = new SalesforceClient(_connectionConfig);
            var properties = typeof(T).GetProperties()
                .Where(p => _descriptor.CanWrite(p.Name));

            var jobj = new JObject();
            foreach (var prop in properties)
            {
                var mappedProperty = _descriptor.GetMappedColumnName(prop.Name);
                var mappedPropertyValue = _descriptor.ProjectPropertyValue(prop.GetValue(obj), prop.Name);

                if (mappedPropertyValue != null)
                    jobj[mappedProperty] = JToken.FromObject(mappedPropertyValue);
            }

            _descriptor.PrepareForPost(jobj);

            var response = await forceClient.CreateAsync(_descriptor.SalesforceTypeName, jobj).ConfigureAwait(false);
            if (!response.Success)
                throw new InvalidOperationException($"Failed to post object: {response.Errors}");

            return response.Id;
        }

        public async Task UpdateAsync(string id, IDictionary<string, object> modifiedProperties)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

            if (modifiedProperties == null || !modifiedProperties.Any())
                throw new ArgumentException("Value contains no elements.", nameof(modifiedProperties));

            var forceClient = new SalesforceClient(_connectionConfig);
            var jobj = new JObject();
            foreach (var modifiedProperty in modifiedProperties)
            {
                var mappedPropertyName = _descriptor.GetMappedColumnName(modifiedProperty.Key);
                jobj.Add(mappedPropertyName, JToken.FromObject(modifiedProperty.Value));
            }

            var sfResponse = await forceClient.UpdateAsync(_descriptor.SalesforceTypeName, id, jobj)
                .ConfigureAwait(false);

            if (!sfResponse.Success)
                throw new InvalidOperationException($"Patch failed: {sfResponse.Errors}");
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

            var forceClient = new SalesforceClient(_connectionConfig);
            var isSuccess = await forceClient.DeleteAsync(_descriptor.SalesforceTypeName, id).ConfigureAwait(false);

            if (!isSuccess)
                throw new InvalidOperationException("Failed to delete object");
        }
    }
}
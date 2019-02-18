using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dinucci.Salesforce.Client.Data;
using LogicMine.DataObject.Filter;
using Newtonsoft.Json.Linq;

namespace LogicMine.DataObject.Salesforce
{
    public class SalesforceObjectStore<T> : IDataObjectStore<T, string>
        where T : new()
    {
        protected IDataApi SalesforceDataApi { get; }
        protected SalesforceObjectDescriptor<T> Descriptor { get; }

        public SalesforceObjectStore(IDataApi salesforceDataApi, SalesforceObjectDescriptor<T> descriptor)
        {
            SalesforceDataApi = salesforceDataApi ?? throw new ArgumentNullException(nameof(salesforceDataApi));
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            if (Descriptor.DataType != typeof(T))
            {
                throw new ArgumentException(
                    $"The store is for '{typeof(T)}' but the supplied descriptor is for '{Descriptor.DataType}'");
            }
        }

        protected virtual void PrepareForCreation(JObject record)
        {
        }

        protected virtual string GetSelectFromQuery(IEnumerable<PropertyInfo> properties)
        {
            var selectList = properties
                .Select(p => Descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            return $"SELECT {selectList} FROM {Descriptor.SalesforceTypeName}";
        }

        protected virtual string GetWhereClause(IFilter<T> filter)
        {
            if (filter?.Terms != null && filter.Terms.Any())
            {
                var filterGenerator =
                    new SalesforceFilterGenerator(filter, Descriptor.GetMappedColumnName);
                var where = filterGenerator.Generate();

                if (string.IsNullOrWhiteSpace(where))
                    throw new InvalidOperationException("Failed to generate WHERE clause");

                return where;
            }

            return string.Empty;
        }

        protected virtual string GetWhereClause(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

            return $"WHERE Id = '{id}'";
        }

        protected virtual T MapObject(JObject record, IEnumerable<PropertyInfo> properties)
        {
            var mappedObject = new T();
            foreach (var property in properties)
            {
                var mappedPropertyName = Descriptor.GetMappedColumnName(property.Name);
                var propValue = record[mappedPropertyName].ToObject(property.PropertyType);
                var sfValue = Descriptor.ProjectColumnValue(propValue, property.PropertyType);

                property.SetValue(mappedObject, sfValue);
            }

            return mappedObject;
        }

        public async Task<T[]> GetCollectionAsync(IFilter<T> filter, int? max = null, int? page = null)
        {
            var properties = Descriptor.GetReadableProperties();

            var query = $"{GetSelectFromQuery(properties)} {GetWhereClause(filter)}";
            if (max.GetValueOrDefault(0) > 0)
            {
                query += $" LIMIT {max}";
                if (page.GetValueOrDefault(0) > 0)
                    query += $" OFFSET {page * max}";
            }

            var queryResult = await SalesforceDataApi.QueryAsync(query).ConfigureAwait(false);
            if (queryResult.Done)
            {
                var result = new T[queryResult.Records.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = MapObject(queryResult.Records[i], properties);

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

            var properties = Descriptor.GetReadableProperties();

            var query = $"{GetSelectFromQuery(properties)} {GetWhereClause(id)}";

            var queryResult = await SalesforceDataApi.QueryAsync(query).ConfigureAwait(false);
            if (queryResult.Done && queryResult.Records.Length == 1)
                return MapObject(queryResult.Records[0], properties);

            throw new InvalidOperationException(
                $"Failed to retrieve {typeof(T)}  - [{query}] - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");
        }

        public async Task<string> CreateAsync(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var properties = typeof(T).GetProperties()
                .Where(p => Descriptor.CanWrite(p.Name));

            var jobj = new JObject();
            foreach (var prop in properties)
            {
                var mappedProperty = Descriptor.GetMappedColumnName(prop.Name);
                var mappedPropertyValue = Descriptor.ProjectPropertyValue(prop.GetValue(obj), prop.Name);

                if (mappedPropertyValue != null)
                    jobj[mappedProperty] = JToken.FromObject(mappedPropertyValue);
            }

            PrepareForCreation(jobj);

            return await SalesforceDataApi.CreateAsync(Descriptor.SalesforceTypeName, jobj).ConfigureAwait(false);
        }

        public async Task UpdateAsync(string id, IDictionary<string, object> modifiedProperties)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

            if (modifiedProperties == null || !modifiedProperties.Any())
                throw new ArgumentException("Value contains no elements.", nameof(modifiedProperties));

            var jobj = new JObject();
            foreach (var modifiedProperty in modifiedProperties)
            {
                var mappedPropertyName = Descriptor.GetMappedColumnName(modifiedProperty.Key);
                jobj.Add(mappedPropertyName, JToken.FromObject(modifiedProperty.Value));
            }

            await SalesforceDataApi.UpdateAsync(Descriptor.SalesforceTypeName, id, jobj).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

            await SalesforceDataApi.DeleteAsync(Descriptor.SalesforceTypeName, id).ConfigureAwait(false);
        }
    }
}
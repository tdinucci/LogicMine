using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using Newtonsoft.Json.Linq;
using Salesforce.Force;

namespace LogicMine.Api.Data.Salesforce
{
    /// <summary>
    /// A base class for a SalesforceLayer which targets a particular data type
    /// </summary>
    public class SalesforceLayer
    {
        protected static readonly object CacheLock = new object();

        protected static Dictionary<Type, IEnumerable<PropertyInfo>> ReadableProperties { get; } =
            new Dictionary<Type, IEnumerable<PropertyInfo>>();

        protected SalesforceLayer()
        {
        }

        protected static IEnumerable<PropertyInfo> GetReadableProperties(Type type, IDbObjectDescriptor descriptor)
        {
            if (ReadableProperties.ContainsKey(type))
                return ReadableProperties[type];

            lock (CacheLock)
            {
                if (ReadableProperties.ContainsKey(type))
                    return ReadableProperties[type];

                var properties = type.GetProperties()
                    .Where(p => descriptor.CanRead(p.Name));

                ReadableProperties.Add(type, properties);

                return properties;
            }
        }
    }

    /// <summary>
    /// A terminal layer which treats Salesforce as an underlying datastore.
    /// </summary>
    /// <typeparam name="T">The data type the layer deals with</typeparam>
    public class SalesforceLayer<T> : SalesforceLayer, IDbLayer<string, T>
        where T : new()
    {
        private readonly IForceClient _forceClient;
        private readonly SalesforceObjectDescriptor _descriptor;

        public SalesforceLayer(IForceClient forceClient, SalesforceObjectDescriptor descriptor)
        {
            _forceClient = forceClient ?? throw new ArgumentNullException(nameof(forceClient));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        /// <summary>
        /// Retreives the requested T from Salesforce and adds it to the baskets AscendPayload
        /// </summary>
        /// <param name="basket">A basket</param>
        /// <param name="visit">The visit the basket is currently making</param>
        /// <returns>A Task that may be awaited</returns>
        public async Task AddResultAsync(IGetBasket<string, T> basket, IVisit visit)
        {
            var properties = GetReadableProperties(typeof(T), _descriptor);
            var selectList = properties
                .Select(p => _descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            var query =
                $"SELECT {selectList} FROM {_descriptor.SalesforceTypeName} WHERE Id = '{basket.DescentPayload}'";

            var result = new T();
            var queryResult = await _forceClient.QueryAsync<JObject>(query).ConfigureAwait(false);
            if (queryResult.Done && queryResult.Records.Count == 1)
            {
                var sfObject = queryResult.Records[0];
                foreach (var property in properties)
                {
                    var mappedPropertyName = _descriptor.GetMappedColumnName(property.Name);
                    var propValue = sfObject[mappedPropertyName].ToObject(property.PropertyType);
                    var sfValue = _descriptor.ProjectColumnValue(propValue, property.PropertyType);

                    property.SetValue(result, sfValue);
                }

                basket.AscentPayload = result;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Query failed [{query}] - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");
            }
        }

        /// <summary>
        /// Retrieves the requested collection of T's from Salesforce and adds them to the baskets AscendPayload
        /// </summary>
        /// <param name="basket">A basket</param>
        /// <param name="visit">The visit the basket is currently making</param>
        /// <returns>A Task that may be awaited</returns>
        public async Task AddResultAsync(IGetCollectionBasket<T> basket, IVisit visit)
        {
            var properties = GetReadableProperties(typeof(T), _descriptor);
            var selectList = properties
                .Select(p => _descriptor.GetMappedColumnName(p.Name))
                .Aggregate((c, n) => $"{c},{n}");

            var query = $"SELECT {selectList} FROM {_descriptor.SalesforceTypeName}";
            if (basket.DescentPayload?.Filter?.Terms != null && basket.DescentPayload.Filter.Terms.Any())
            {
                var filterGenerator =
                    new SalesforceFilterGenerator(basket.DescentPayload.Filter, _descriptor.GetMappedColumnName);
                var where = filterGenerator.Generate();

                if (string.IsNullOrWhiteSpace(where))
                    throw new InvalidOperationException("Failed to generate WHERE clause");

                query += $" {where}";
            }

            if (basket.DescentPayload?.Max.GetValueOrDefault(0) > 0)
            {
                query += $" LIMIT {basket.DescentPayload.Max}";
                if (basket.DescentPayload?.Page.GetValueOrDefault(0) > 0)
                    query += $" OFFSET {basket.DescentPayload.Page * basket.DescentPayload.Max}";
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

                basket.AscentPayload = result;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Query failed [{query}] - Done: {queryResult.Done}, Count: {queryResult.TotalSize}");
            }
        }

        /// <summary>
        /// Inserts the requested T into Salesforce and sets the baskets AscentPayload to new records identity
        /// </summary>
        /// <param name="basket">A basket</param>
        /// <param name="visit">The visit the basket is currently making</param>
        /// <returns>A Task that may be awaited</returns>
        public async Task AddResultAsync(IPostBasket<T, string> basket, IVisit visit)
        {
            var toCreate = basket.DescentPayload;
            if (toCreate == null)
                throw new InvalidOperationException("There is no object to create in the basket");

            var properties = typeof(T).GetProperties()
                .Where(p => _descriptor.CanWrite(p.Name));

            var jobj = new JObject();
            foreach (var prop in properties)
            {
                var mappedProperty = _descriptor.GetMappedColumnName(prop.Name);
                var mappedPropertyValue = _descriptor.ProjectPropertyValue(prop.GetValue(toCreate), prop.Name);

                if (mappedPropertyValue != null)
                    jobj[mappedProperty] = JToken.FromObject(mappedPropertyValue);
            }

            _descriptor.PrepareForPost(jobj);

            var response = await _forceClient.CreateAsync(_descriptor.SalesforceTypeName, jobj).ConfigureAwait(false);
            if (!response.Success)
                throw new InvalidOperationException($"Failed to post object: {response.Errors}");

            basket.AscentPayload = response.Id;
        }

        /// <summary>
        /// Performs a partial update to a T in Salesforce and sets the baskets AscentPayload to the number of records affected
        /// </summary>
        /// <param name="basket">A basket</param>
        /// <param name="visit">The visit the basket is currently making</param>
        /// <returns>A Task that may be awaited</returns>
        public async Task AddResultAsync(IPatchBasket<string, T, int> basket, IVisit visit)
        {
            var delta = basket.DescentPayload?.Delta;
            if (delta == null)
                throw new InvalidOperationException("Basket does not contain a delta");

            if (string.IsNullOrWhiteSpace(delta.Identity))
                throw new InvalidOperationException("The delta contains no id");

            if (delta.ModifiedProperties == null || !delta.ModifiedProperties.Any())
                throw new InvalidOperationException("The delta contains no properties");

            var jobj = new JObject();
            foreach (var modifiedProperty in delta.ModifiedProperties)
            {
                var mappedPropertyName = _descriptor.GetMappedColumnName(modifiedProperty.Key);
                jobj.Add(mappedPropertyName, JToken.FromObject(modifiedProperty.Value));
            }

            var success = await _forceClient.UpdateAsync(_descriptor.SalesforceTypeName, delta.Identity, jobj)
                .ConfigureAwait(false);

            if (!success.Success)
                throw new InvalidOperationException($"Patch failed: {success.Errors}");

            // show 1 record was patched
            basket.AscentPayload = 1;
        }

        /// <summary>
        /// Deletes a T from Salesforce and sets the baskets AscentPayload to the number of records affected
        /// </summary>
        /// <param name="basket">A basket</param>
        /// <param name="visit">The visit the basket is currently making</param>
        /// <returns>A Task that may be awaited</returns>
        public async Task AddResultAsync(IDeleteBasket<string, int> basket, IVisit visit)
        {
            if (string.IsNullOrWhiteSpace(basket.DescentPayload))
                throw new InvalidOperationException("Basket does not contain an id");

            var success = await _forceClient.DeleteAsync(_descriptor.SalesforceTypeName, basket.DescentPayload)
                .ConfigureAwait(false);

            if (!success)
                throw new InvalidOperationException("Failed to delete object");

            // show 1 record was deleted
            basket.AscentPayload = 1;
        }

        /// <summary>
        /// This operation is not supported
        /// </summary>
        /// <param name="basket"></param>
        /// <param name="visit"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">An exception of this type is always thrown</exception>
        public Task AddResultAsync(IDeleteCollectionBasket<T, int> basket, IVisit visit)
        {
            throw new NotSupportedException("Deleting collections is not supported");
        }
    }
}
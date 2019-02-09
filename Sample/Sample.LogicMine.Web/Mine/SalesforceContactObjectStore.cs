using System;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.Salesforce;
using Newtonsoft.Json.Linq;

namespace Sample.LogicMine.Web.Mine
{
    public abstract class SalesforceContactObjectStore<T> : SalesforceObjectStore<T> where T : new()
    {
        protected abstract string SalesforceContactType { get; }
        
        protected string ContactTypeId { get; private set; }

        public SalesforceContactObjectStore(SalesforceCredentials credentials, SalesforceObjectDescriptor<T> descriptor)
            : base(credentials, descriptor)
        {
            InitialiseRecordTypeIds();
        }

        protected override void PrepareForCreation(JObject record)
        {
            record["Salutation"] = ".";
            record["RecordTypeId"] = ContactTypeId;
        }
        
        private void InitialiseRecordTypeIds()
        {
            var query = "SELECT Id FROM RecordType " +
                        $"WHERE SObjectType = 'Contact' AND Name = '{SalesforceContactType}' " +
                        "AND IsActive = true";

            var sfClient = new SalesforceClient(Credentials);
            var result = sfClient.QueryAsync(query).GetAwaiter().GetResult();

            if (result.Done && result.Records.Count == 1)
                ContactTypeId = result.Records[0]["Id"].Value<string>();

            if (string.IsNullOrWhiteSpace(ContactTypeId))
            {
                throw new InvalidOperationException(
                    $"Failed to initialise Salesforce record type for '{SalesforceContactType}'");
            }
        }

        protected override string GetWhereClause(IFilter<T> filter)
        {
            return GetRestrictedWhereClause(base.GetWhereClause(filter));
        }

        protected override string GetWhereClause(string id)
        {
            return GetRestrictedWhereClause(base.GetWhereClause(id));
        }

        private string GetRestrictedWhereClause(string baseWhereClause)
        {
            var condition = $"RecordTypeId = '{ContactTypeId}'";

            var clause = baseWhereClause ?? string.Empty;
            if (string.IsNullOrWhiteSpace(clause))
                clause += "WHERE";
            else
                clause += " AND";

            return $"{clause} {condition}";
        }
    }
}
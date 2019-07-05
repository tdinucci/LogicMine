using System;
using System.Collections.Generic;
using System.Linq;
using LogicMine.DataObject.Filter;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LogicMine.DataObject.MongoDb
{
    /// <summary>
    /// Used to convert IFilters to MongoDb filters
    /// </summary>
    public class MongoDbFilterGenerator<T>
    {
        private readonly IFilter<T> _filter;
        private readonly Func<string, string> _covertPropToColumnName;

        /// <summary>
        /// Construct a new MongoDbFilterGenerator
        /// </summary>
        /// <param name="filter">The IFilter to convert</param>
        /// <param name="covertPropToFieldName">A function that can map IFilter term properties to document field names</param>
        public MongoDbFilterGenerator(IFilter<T> filter, Func<string, string> covertPropToFieldName)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _covertPropToColumnName = covertPropToFieldName;
        }

        /// <summary>
        /// Generate a MongoDb filter based on the filter passed to the constructor
        /// </summary>
        /// <returns></returns>
        public FilterDefinition<T> Generate()
        {
            if (_filter.Terms != null && _filter.Terms.Any())
            {
                var conditions = new List<FilterDefinition<T>>();
                foreach (var term in _filter.Terms)
                    conditions.Add(GenerateCondition(term));

                return Builders<T>.Filter.And(conditions);
            }

            return new BsonDocument();
        }

        private FilterDefinition<T> GenerateCondition(IFilterTerm term)
        {
            switch (term.Operator)
            {
                case FilterOperators.Range:
                    return GenerateBetweenCondition((RangeFilterTerm) term);
                case FilterOperators.In:
                    return GenerateInCondition((InFilterTerm) term);
                default:
                    return GenerateBasicCondition(term);
            }
        }

        private FilterDefinition<T> GenerateBetweenCondition(RangeFilterTerm term)
        {
            var fieldName = GetColumnName(term.PropertyName);
            var fromValue = GetParameterStringValue(term.From);
            var toValue = GetParameterStringValue(term.To);

            return Builders<T>.Filter.And
            (
                Builders<T>.Filter.Gte(fieldName, fromValue),
                Builders<T>.Filter.Lte(fieldName, toValue)
            );
        }

        private FilterDefinition<T> GenerateInCondition(InFilterTerm term)
        {
            var fieldName = GetColumnName(term.PropertyName);
            var values = term.Value.Select(GetParameterStringValue);

            return Builders<T>.Filter.In(fieldName, values);
        }

        private FilterDefinition<T> GenerateBasicCondition(IFilterTerm term)
        {
            var fieldName = GetColumnName(term.PropertyName);
            var clause = fieldName + " ";
            var paramValue = term.Value;
            switch (term.Operator)
            {
                case FilterOperators.Equal:
                    return Builders<T>.Filter.Eq(fieldName, paramValue);
                case FilterOperators.NotEqual:
                    return Builders<T>.Filter.Ne(fieldName, paramValue);
                case FilterOperators.LessThan:
                    return Builders<T>.Filter.Lt(fieldName, paramValue);
                case FilterOperators.LessThanOrEqual:
                    return Builders<T>.Filter.Lte(fieldName, paramValue);
                case FilterOperators.GreaterThan:
                    return Builders<T>.Filter.Gt(fieldName, paramValue);
                case FilterOperators.GreaterThanOrEqual:
                    return Builders<T>.Filter.Gte(fieldName, paramValue);
                case FilterOperators.StartsWith:
                    return Builders<T>.Filter.Regex(fieldName, new BsonRegularExpression($"^{paramValue}"));
                case FilterOperators.EndsWith:
                    return Builders<T>.Filter.Regex(fieldName, new BsonRegularExpression($"{paramValue}$"));
                case FilterOperators.Contains:
                    return Builders<T>.Filter.Regex(fieldName, new BsonRegularExpression($"{paramValue}"));
            }

            return clause;
        }

        private string GetColumnName(string propName)
        {
            if (_covertPropToColumnName == null)
                return propName;

            var field = _covertPropToColumnName(propName);
            if (string.IsNullOrWhiteSpace(field))
                throw new InvalidOperationException($"Could not find field mapped to '{propName}'");

            return field;
        }

        private string GetParameterStringValue(object value)
        {
            if (value == null)
                return "null";

            if (value is DateTime time)
                return $"ISODate('{time:yyyy-MM-ddTHH:mm:ssZ}')";

            return value.ToString();
        }
    }
}
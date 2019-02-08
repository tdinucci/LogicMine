using System;
using System.Linq;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject.Salesforce
{
    /// <summary>
    /// Used to convert IFilters to SOQL WHERE clauses suitable for Salesforce
    /// </summary>
    public class SalesforceFilterGenerator
    {
        private readonly IFilter _filter;
        private readonly Func<string, string> _covertPropToColumnName;

        /// <summary>
        /// Construct a new SalesforceFilterGenerator
        /// </summary>
        /// <param name="filter">The IFilter to convert</param>
        /// <param name="covertPropToFieldName">A function that can map IFilter term properties to database column names</param>
        public SalesforceFilterGenerator(IFilter filter, Func<string, string> covertPropToFieldName)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _covertPropToColumnName = covertPropToFieldName;
        }

        /// <summary>
        /// Generate the actual WHERE SOQL clause
        /// </summary>
        /// <returns>A WHERE SOQL clause which represents the IFilter that was provided at construction time</returns>
        public string Generate()
        {
            const string and = " AND ";
            var clause = string.Empty;
            if (_filter.Terms != null && _filter.Terms.Any())
            {
                foreach (var term in _filter.Terms)
                {
                    var condition = GenerateCondition(term);
                    clause += $"{condition}{and}";
                }

                clause = "WHERE " + clause.Substring(0, clause.Length - and.Length);
            }

            return clause;
        }

        private string GenerateCondition(IFilterTerm term)
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

        private string GenerateBetweenCondition(RangeFilterTerm term)
        {
            var fieldName = GetColumnName(term.PropertyName);
            var fromValue = GetParameterStringValue(term.From);
            var toValue = GetParameterStringValue(term.To);

            var clause = $"{fieldName} >= {fromValue} AND {fieldName} <= {toValue}";
            return clause;
        }

        private string GenerateInCondition(InFilterTerm term)
        {
            var clause = string.Empty;
            var fieldName = GetColumnName(term.PropertyName);
            var containsNull = false;
            foreach (var item in term.Value)
            {
                if (item == null)
                    containsNull = true;
                else
                {
                    var paramValue = GetParameterStringValue(item);
                    clause += $"{paramValue},";
                }
            }

            clause = $"{fieldName} IN ({clause.TrimEnd(',')})";

            if (containsNull)
                clause = $"({fieldName} = NULL OR {clause})";

            return clause;
        }

        private string GenerateBasicCondition(IFilterTerm term)
        {
            var fieldName = GetColumnName(term.PropertyName);
            var clause = fieldName + " ";
            var paramValue = GetParameterStringValue(term.Value);
            switch (term.Operator)
            {
                case FilterOperators.Equal:
                    clause += $"= {paramValue}";
                    break;
                case FilterOperators.NotEqual:
                    clause += $"!= {paramValue}";
                    break;
                case FilterOperators.LessThan:
                    clause += $"< {paramValue}";
                    break;
                case FilterOperators.LessThanOrEqual:
                    clause += $"<= {paramValue}";
                    break;
                case FilterOperators.GreaterThan:
                    clause += $"> {paramValue}";
                    break;
                case FilterOperators.GreaterThanOrEqual:
                    clause += $">= {paramValue}";
                    break;
                case FilterOperators.StartsWith:
                    clause += $"LIKE {paramValue} + '%'";
                    break;
                case FilterOperators.EndsWith:
                    clause += $"LIKE '%' + {paramValue}";
                    break;
                case FilterOperators.Contains:
                    clause += $"LIKE '%' + {paramValue} + '%'";
                    break;
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

            if (value is string)
                return $"'{value}'";

            if (value is DateTime time)
                return time.ToString("yyyy-MM-ddTHH:mm:ssZ");

            return value.ToString();
        }
    }
}
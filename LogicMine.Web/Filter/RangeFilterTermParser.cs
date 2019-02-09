using System;
using System.Collections.Generic;
using FastMember;
using LogicMine.DataObject.Filter;

namespace LogicMine.Web.Filter
{
    /// <summary>
    /// Used for parsing a string which is expected to contain "range" term and produces InFilterTerm objects which are bound to T
    /// </summary>
    public class RangeFilterTermParser<T> : RangeFilterTermParser
    {
        private readonly IDictionary<string, Member> _typeMembers;

        /// <summary>
        /// The type the filter will be bound to
        /// </summary>
        public Type Type => typeof(T);

        /// <summary>
        /// Construct a new InFilterTermParser
        /// </summary>
        /// <param name="field">The field which the "in" term applies to</param>
        /// <param name="inExpression">The string representation of the values</param>
        /// <param name="typeMembers">Description of the properties on T</param>
        public RangeFilterTermParser(string field, string inExpression, IDictionary<string, Member> typeMembers) :
            base(field, inExpression)
        {
            if (typeMembers == null || typeMembers.Count == 0)
                throw new ArgumentException("Value cannot be null or an empty collection.", nameof(typeMembers));

            _typeMembers = typeMembers;
        }

        /// <inheritdoc />
        public override RangeFilterTerm Parse()
        {
            return ConvertToTypedTerm(base.Parse());
        }

        private RangeFilterTerm ConvertToTypedTerm(RangeFilterTerm term)
        {
            var field = term.PropertyName.ToLower();
            if (!_typeMembers.ContainsKey(field))
                throw new InvalidOperationException(
                    $"No public member on {Type} called {field} - case insensitive search");

            var member = _typeMembers[field];
            var fromValue = StringConverter.FromString((string) term.From, member.Type);
            var toValue = StringConverter.FromString((string) term.To, member.Type);

            return new RangeFilterTerm(member.Name, fromValue, toValue);
        }
    }

    /// <summary>
    /// Used for parsing a string which is expected to contain "range" term and produces InFilterTerm objects
    /// </summary>
    public class RangeFilterTermParser : IFilterTermParser
    {
        private readonly string _field;
        private readonly string _rangeExpression;

        /// <summary>
        /// Construct a new RangeFilterTermParser
        /// </summary>
        /// <param name="field">The field which the "in" term applies to</param>
        /// <param name="rangeExpression">The string representation of the values in the range</param>
        public RangeFilterTermParser(string field, string rangeExpression)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(field));

            if (string.IsNullOrWhiteSpace(rangeExpression))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(rangeExpression));

            _field = field.Trim();
            _rangeExpression = rangeExpression.Trim();
        }

        /// <summary>
        /// Parses the provided filter term, generating an RangeFilterTerm
        /// </summary>
        /// <returns>A RangeFilterTerm</returns>
        public virtual RangeFilterTerm Parse()
        {
            var values = GetRange();
            return new RangeFilterTerm(_field, values.Item1, values.Item2);
        }

        IFilterTerm IFilterTermParser.Parse()
        {
            return Parse();
        }

        private Tuple<string, string> GetRange()
        {
            const string delimiter = "..";
            var toIndex = _rangeExpression.IndexOf(delimiter, StringComparison.Ordinal);

            if (toIndex < 0)
                throw new InvalidOperationException(
                    $"'{FilterOperators.Range}' filter does not contain '..' delimiter");

            var lvalueString = _rangeExpression.Substring(0, toIndex).Trim();
            var rvalueString = _rangeExpression.Substring(toIndex + delimiter.Length).Trim();

            if (string.IsNullOrEmpty(lvalueString) || string.IsNullOrWhiteSpace(rvalueString))
                throw new InvalidOperationException($"'{FilterOperators.Range}' filter is missing one or both values");

            return new Tuple<string, string>(lvalueString, rvalueString);
        }
    }
}
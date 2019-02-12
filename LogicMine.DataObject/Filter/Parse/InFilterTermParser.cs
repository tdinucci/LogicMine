using System;
using System.Collections.Generic;
using System.Linq;
using FastMember;

namespace LogicMine.DataObject.Filter.Parse
{
    /// <summary>
    /// Used for parsing a string which is expected to contain "in" terms and produces InFilterTerm objects which are bound to T
    /// </summary>
    public class InFilterTermParser<T> : InFilterTermParser
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
        public InFilterTermParser(string field, string inExpression, IDictionary<string, Member> typeMembers) :
            base(field, inExpression)
        {
            if (typeMembers == null || typeMembers.Count == 0)
                throw new ArgumentException("Value cannot be null or an empty collection.", nameof(typeMembers));

            _typeMembers = typeMembers;
        }

        /// <inheritdoc />
        public override InFilterTerm Parse()
        {
            return ConvertToTypedTerm(base.Parse());
        }

        private InFilterTerm ConvertToTypedTerm(InFilterTerm term)
        {
            var field = term.PropertyName.ToLower();
            if (!_typeMembers.ContainsKey(field))
                throw new InvalidOperationException(
                    $"No public member on {Type} called {field} - case insensitive search");

            var member = _typeMembers[field];
            var inValues = term.Value
                .Select(s => StringConverter.FromString((string) s, member.Type));

            return new InFilterTerm(member.Name, inValues);
        }
    }

    /// <summary>
    /// Used for parsing a string which is expected to contain "in" terms and produces InFilterTerm objects
    /// </summary>
    public class InFilterTermParser : IFilterTermParser
    {
        private readonly string _field;
        private readonly string _inExpression;

        /// <summary>
        /// Construct a new InFilterTermParser
        /// </summary>
        /// <param name="field">The field which the "in" term applies to</param>
        /// <param name="inExpression">The string representation of the values</param>
        public InFilterTermParser(string field, string inExpression)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(field));
            if (string.IsNullOrWhiteSpace(inExpression))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inExpression));

            _field = field.Trim();
            _inExpression = inExpression.Trim();
        }

        /// <summary>
        /// Parses the provided filter term, generating an InFilterTerm
        /// </summary>
        /// <returns>An InFilterTerm</returns>
        public virtual InFilterTerm Parse()
        {
            return new InFilterTerm(_field, GetInFilterStringValues());
        }

        IFilterTerm IFilterTermParser.Parse()
        {
            return Parse();
        }

        private IEnumerable<string> GetInFilterStringValues()
        {
            if (!_inExpression.StartsWith("(") || !_inExpression.EndsWith(")"))
            {
                throw new InvalidOperationException(
                    $"'{FilterOperators.In}' term is invalid, it is expected that elements are surrounded by parentheses");
            }

            var result = _inExpression
                .TrimStart('(')
                .TrimEnd(')')
                .Split(',')
                .Select(vs => vs.Trim())
                .ToArray();

            if (result.Length == 1 && result[0] == string.Empty)
            {
                throw new InvalidOperationException(
                    $"'{FilterOperators.In}' term is invalid, it is expected that there is at least 1 element");
            }

            return result;
        }
    }
}
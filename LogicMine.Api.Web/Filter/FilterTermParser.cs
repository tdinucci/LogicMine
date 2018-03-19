/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using FastMember;
using LogicMine.Api.Filter;

namespace LogicMine.Api.Web.Filter
{
  /// <summary>
  /// <para>Parses string terms and produces instances of IFilterTerm which are bound to T</para>
  /// <para>Example string terms are:</para>
  /// 
  /// <para>MyStringField eq 'hello'</para>
  /// <para>MyIntField lt 100</para>
  /// <para>MyIntField in (1, 2, 3, 4)</para>
  /// <para>MyDateField from '2017-01-01' to '2017-12-31'</para>
  /// </summary>
  public class FilterTermParser<T> : FilterTermParser
  {
    private readonly IDictionary<string, Member> _typeMembers;

    /// <summary>
    /// The type the term applies to
    /// </summary>
    public Type Type => typeof(T);

    /// <summary>
    /// Construct a new FilterTermParser
    /// </summary>
    /// <param name="term">The term to parse</param>
    /// <param name="typeMembers">Description of the properties on T</param>
    public FilterTermParser(string term, IDictionary<string, Member> typeMembers) :
      base(term)
    {
      if (typeMembers == null || typeMembers.Count == 0)
        throw new ArgumentException("Value cannot be null or an empty collection.", nameof(typeMembers));

      _typeMembers = typeMembers;
    }

    /// <inheritdoc />
    protected override InFilterTerm CreateInFilterTerm(string fieldName, string valueAsString)
    {
      EnsureCanFilter(fieldName);
      return new InFilterTermParser<T>(fieldName, valueAsString, _typeMembers).Parse();
    }

    /// <inheritdoc />
    protected override RangeFilterTerm CreateRangeFilterTerm(string fieldName, string valueAsString)
    {
      EnsureCanFilter(fieldName);
      return new RangeFilterTermParser<T>(fieldName, valueAsString, _typeMembers).Parse();
    }

    /// <inheritdoc />
    protected override FilterTerm CreateBasicFilterTerm(string fieldName, FilterOperators op, string valueAsString)
    {
      EnsureCanFilter(fieldName);
      var term = base.CreateBasicFilterTerm(fieldName, op, valueAsString);
      return ConvertToTypedTerm(term);
    }

    private FilterTerm ConvertToTypedTerm(IFilterTerm term)
    {
      var member = GetTypeMember(term.PropertyName);

      EnsureValidOperator(term.Operator, member.Type);

      var value = StringConverter.FromString((string) term.Value, member.Type);

      return new FilterTerm(member.Name, term.Operator, value);
    }

    private Member GetTypeMember(string fieldName)
    {
      fieldName = fieldName.ToLower();
      if (!_typeMembers.ContainsKey(fieldName))
        throw new InvalidOperationException($"No public member on {Type} called {fieldName} - case insensitive search");

      return _typeMembers[fieldName];
    }

    private void EnsureCanFilter(string fieldName)
    {
      var member = GetTypeMember(fieldName);
      if (!StringConverter.IsConvertable(member.Type))
        throw new InvalidOperationException($"'{fieldName}' is not of a filterable type");
    }

    private void EnsureValidOperator(FilterOperators op, Type fieldType)
    {
      if (fieldType == typeof(bool))
      {
        if (op != FilterOperators.Equal && op != FilterOperators.NotEqual)
          throw new InvalidOperationException($"The '{op}' filter is not compatible with boolean fields");
      }
      else if (fieldType == typeof(string))
      {
        if (op == FilterOperators.LessThan || op == FilterOperators.LessThanOrEqual ||
            op == FilterOperators.GreaterThan || op == FilterOperators.GreaterThanOrEqual ||
            op == FilterOperators.Range)
        {
          throw new InvalidOperationException($"The '{op}' filter is not compatible with string fields");
        }
      }
      else if (op == FilterOperators.StartsWith || op == FilterOperators.Contains || op == FilterOperators.EndsWith)
      {
        throw new InvalidOperationException($"The '{op}' filter is only compatible with string fields");
      }
    }
  }

  /// <summary>
  /// <para>Parses string terms and produces instances of IFilterTerm.</para>
  /// <para>Example string terms are:</para>
  /// 
  /// <para>MyStringField eq 'hello'</para>
  /// <para>MyIntField lt 100</para>
  /// <para>MyIntField in (1, 2, 3, 4)</para>
  /// <para>MyDateField from '2017-01-01' to '2017-12-31'</para>
  /// </summary>
  public class FilterTermParser : IFilterTermParser
  {
    /// <summary>
    /// The string representation of the term
    /// </summary>
    public string Term { get; }

    /// <summary>
    /// Construct a new FilterTermParser
    /// </summary>
    /// <param name="term">The string representation of the term</param>
    public FilterTermParser(string term)
    {
      if (string.IsNullOrWhiteSpace(term))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(term));

      Term = term;
    }

    /// <summary>
    /// Parses the string filter term representation
    /// </summary>
    /// <returns>an IFilterTerm which is the result of parsing the string term provided at construction time</returns>
    public virtual IFilterTerm Parse()
    {
      var tokens = GetTokens();

      var fieldName = tokens.Item1;
      var op = GetFilterOperator(tokens.Item2);

      switch (op)
      {
        case FilterOperators.In:
          return CreateInFilterTerm(fieldName, tokens.Item3);
        case FilterOperators.Range:
          return CreateRangeFilterTerm(fieldName, tokens.Item3);
        default:
          return CreateBasicFilterTerm(fieldName, op, tokens.Item3);
      }
    }

    /// <summary>
    /// Generates an InFilterTerm
    /// </summary>
    /// <param name="fieldName">The filtered field</param>
    /// <param name="valueAsString">The filtered value</param>
    /// <returns>An InFilterTerm</returns>
    protected virtual InFilterTerm CreateInFilterTerm(string fieldName, string valueAsString)
    {
      return new InFilterTermParser(fieldName, valueAsString).Parse();
    }

    /// <summary>
    /// Generates a RangeFilterTermParser
    /// </summary>
    /// <param name="fieldName">The filtered field</param>
    /// <param name="valueAsString">The filtered values</param>
    /// <returns>An RangeFilterTermParser</returns>
    protected virtual RangeFilterTerm CreateRangeFilterTerm(string fieldName, string valueAsString)
    {
      return new RangeFilterTermParser(fieldName, valueAsString).Parse();
    }

    /// <summary>
    /// Generates a FilterTerm
    /// </summary>
    /// <param name="fieldName">The filtered field</param>
    /// <param name="op">The filter operator</param>
    /// <param name="valueAsString">The filtered values</param>
    /// <returns>An FilterTerm</returns>
    protected virtual FilterTerm CreateBasicFilterTerm(string fieldName, FilterOperators op, string valueAsString)
    {
      return new FilterTerm(fieldName, op, valueAsString);
    }

    private Tuple<string, string, string> GetTokens()
    {
      var tokens = Term.Split(new[] {' '}, 3, StringSplitOptions.RemoveEmptyEntries);
      if (tokens.Length != 3)
        throw new InvalidOperationException($"Invalid filter term '{Term}'");

      return new Tuple<string, string, string>(tokens[0].Trim(), tokens[1].Trim(), tokens[2].Trim());
    }

    private FilterOperators GetFilterOperator(string opString)
    {
      switch (opString.ToLower())
      {
        case "eq":
          return FilterOperators.Equal;
        case "ne":
          return FilterOperators.NotEqual;
        case "lt":
          return FilterOperators.LessThan;
        case "le":
          return FilterOperators.LessThanOrEqual;
        case "gt":
          return FilterOperators.GreaterThan;
        case "ge":
          return FilterOperators.GreaterThanOrEqual;
        case "startswith":
          return FilterOperators.StartsWith;
        case "endswith":
          return FilterOperators.EndsWith;
        case "contains":
          return FilterOperators.Contains;
        case "in":
          return FilterOperators.In;
        case "range":
          return FilterOperators.Range;
        default:
          throw new InvalidOperationException($"Unexpected filter operator: '{opString}'");
      }
    }
  }
}

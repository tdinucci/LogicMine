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
using System.Linq;
using FastMember;
using LogicMine.Api.Filter;

namespace LogicMine.Api.Web.Filter
{
  /// <summary>
  /// <para>
  /// Parses string filter expressions and produces instances of IFilter which is bound to T.
  /// </para>
  /// <para>Example filter expressions are:</para>
  /// <para>MyStringField in ('one', 'two', 'three')</para>
  /// <para>MyStringField in ('one', 'two', 'three') and MyIntField from 1 to 10 and MyDateField lt '2018-06-25'</para>
  /// </summary>
  public class FilterParser<T> : FilterParser
  {
    /// <summary>
    /// The type this filter is bound to
    /// </summary>
    public Type Type => typeof(T);

    /// <summary>
    /// Construct a new FilterParser
    /// </summary>
    /// <param name="expression">The string representation of the filter</param>
    public FilterParser(string expression) : base(expression)
    {
    }

    /// <summary>
    /// Generates an IFilter bound to T from the expression provided at construction time
    /// </summary>
    /// <returns>An IFilter</returns>
    public new IFilter<T> Parse()
    {
      if (!(base.Parse() is IFilter<T> filter))
        throw new InvalidOperationException($"Failed to generate filtered for {Type}");

      return filter;
    }

    /// <summary>
    /// Generates an IFilter which contains IFilterTerms which are the product of parsing termExpressions. 
    /// While the terms are parsed they are tested to be valid against T, i.e. names map to properties and the 
    /// types are compatible.
    /// </summary>
    /// <param name="termExpressions">String representations of filter terms</param>
    /// <returns>An IFilter</returns>
    protected override IFilter CreateFilter(IEnumerable<string> termExpressions)
    {
      // TODO: consider caching
      var members = TypeAccessor.Create(Type).GetMembers().ToDictionary(m => m.Name.ToLower(), m => m);

      var terms = termExpressions.Select(t => new FilterTermParser<T>(t, members).Parse()).ToArray();
      return new Filter<T>(terms);
    }
  }

  /// <summary>
  /// <para>
  /// Parses string filter expressions and produces instances of IFilter which is bound to T.
  /// </para>
  /// <para>Example filter expressions are:</para>
  /// <para>MyStringField in ('one', 'two', 'three')</para>
  /// <para>MyStringField in ('one', 'two', 'three') and MyIntField from 1 to 10 and MyDateField lt '2018-06-25'</para>
  /// </summary>
  public class FilterParser : IFilterParser
  {
    // Or filters are not supported because they open a potential world of pain when it comes to performance
    private const string AndDelimiter = "and";

    /// <summary>
    /// The string representation of the filter
    /// </summary>
    public string Expression { get; }

    /// <summary>
    /// Construct a new FilterParser
    /// </summary>
    /// <param name="expression">The string representation of the filter</param>
    public FilterParser(string expression)
    {
      if (string.IsNullOrWhiteSpace(expression))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(expression));

      Expression = expression;
    }

    /// <inheritdoc />
    public IFilter Parse()
    {
      var tokens = Expression.Split(null);
      var termExpressions = new List<string>();

      var termExpression = string.Empty;
      foreach (var token in tokens)
      {
        if (string.Equals(token, AndDelimiter, StringComparison.CurrentCultureIgnoreCase))
        {
          termExpressions.Add(termExpression.Trim());
          termExpression = string.Empty;
        }
        else
          termExpression += token + " ";
      }

      if (string.IsNullOrWhiteSpace(termExpression))
        throw new InvalidOperationException("Empty term expression encountered");

      termExpressions.Add(termExpression);

      return CreateFilter(termExpressions);
    }

    /// <summary>
    /// Generates an IFilter which contains IFilterTerms which are the product of parsing termExpressions
    /// </summary>
    /// <param name="termExpressions">String representations of filter terms</param>
    /// <returns>An IFilter</returns>
    protected virtual IFilter CreateFilter(IEnumerable<string> termExpressions)
    {
      var terms = termExpressions.Select(t => new FilterTermParser(t).Parse()).ToArray();
      return new Api.Filter.Filter(terms);
    }
  }
}
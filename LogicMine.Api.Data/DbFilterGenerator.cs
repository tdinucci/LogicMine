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
using System.Data;
using System.Linq;
using LogicMine.Api.Filter;

namespace LogicMine.Api.Data
{
  /// <summary>
  /// Used to convert an IFilter to a IDbFilter
  /// </summary>
  /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
  public abstract class DbFilterGenerator<TDbParameter>
    where TDbParameter : IDbDataParameter
  {
    private readonly IFilter _filter;
    private readonly Func<string, string> _covertPropToColumnName;

    private int _paramNo;

    /// <summary>
    /// Construct a new DbFilterGenerator
    /// </summary>
    /// <param name="filter">The IFilter to convert</param>
    protected DbFilterGenerator(IFilter filter) : this(filter, null)
    {
    }

    /// <summary>
    /// Construct a new DbFilterGenerator
    /// </summary>
    /// <param name="filter">The IFilter to convert</param>
    /// <param name="covertPropToFieldName">A function that can map IFilter term properties to database column names</param>
    protected DbFilterGenerator(IFilter filter, Func<string, string> covertPropToFieldName)
    {
      _filter = filter ?? throw new ArgumentNullException(nameof(filter));
      _covertPropToColumnName = covertPropToFieldName;
    }

    /// <summary>
    /// Returns a new parameter instance for the particular database type used
    /// </summary>
    /// <param name="name">The parameter name</param>
    /// <param name="value">The parameter value</param>
    /// <returns>A parameter for the particular database type used</returns>
    protected abstract TDbParameter GetDbParameter(string name, object value);

    /// <summary>
    /// Returns a new IDbFilter instance suitable for the targetted database type
    /// </summary>
    /// <param name="whereClause">The flters WHERE clause</param>
    /// <param name="parameters">The parameters which participate in the WHERE clause</param>
    /// <returns></returns>
    protected abstract IDbFilter<TDbParameter> GetDbFilter(string whereClause, TDbParameter[] parameters);

    /// <summary>
    /// Generate the actual IDbFilter
    /// </summary>
    /// <returns>An IDbFilter which represents the IFilter that was provided at construction time</returns>
    public IDbFilter<TDbParameter> Generate()
    {
      const string and = " AND ";
      var clause = string.Empty;
      var parameters = new List<TDbParameter>();
      if (_filter.Terms != null && _filter.Terms.Any())
      {
        foreach (var term in _filter.Terms)
        {
          var condition = GenerateCondition(term);
          clause += $"{condition.Item1}{and}";
          if (condition.Item2.Count > 0)
            parameters.AddRange(condition.Item2);
        }

        clause = "WHERE " + clause.Substring(0, clause.Length - and.Length);
      }

      return GetDbFilter(clause, parameters.ToArray());
    }

    private Tuple<string, IList<TDbParameter>> GenerateCondition(IFilterTerm term)
    {
      switch (term.Operator)
      {
        case FilterOperators.Range:
          return GenerateBetweenCondition((RangeFilterTerm) term);
        case FilterOperators.In:
          return GenerateInCondition((InFilterTerm) term);
        default:
          var condition = GenerateBasicCondition(term);
          var parameters = new List<TDbParameter>();
          if (condition.Item2 != null)
            parameters.Add(condition.Item2);

          return new Tuple<string, IList<TDbParameter>>(condition.Item1, parameters);
      }
    }

    private Tuple<string, IList<TDbParameter>> GenerateBetweenCondition(RangeFilterTerm term)
    {
      var fieldName = GetColumnName(term.PropertyName);
      var p1Name = GetParameterName();
      var p2Name = GetParameterName();
      var clause = $"{fieldName} BETWEEN {p1Name} AND {p2Name}";
      var parameters = new List<TDbParameter>
      {
        GetDbParameter(p1Name, term.From),
        GetDbParameter(p2Name, term.To)
      };

      return new Tuple<string, IList<TDbParameter>>(clause, parameters);
    }

    private Tuple<string, IList<TDbParameter>> GenerateInCondition(InFilterTerm term)
    {
      var clause = string.Empty;
      var parameters = new List<TDbParameter>();
      var fieldName = GetColumnName(term.PropertyName);
      var containsNull = false;
      foreach (var item in term.Value)
      {
        if (item == null)
          containsNull = true;
        else
        {
          var paramName = GetParameterName();
          parameters.Add(GetDbParameter(paramName, item));
          clause += $"{paramName},";
        }
      }

      clause = $"{fieldName} IN ({clause.TrimEnd(',')})";

      if (containsNull)
        clause = $"({fieldName} IS NULL OR {clause})";

      return new Tuple<string, IList<TDbParameter>>(clause, parameters);
    }

    private Tuple<string, TDbParameter> GenerateBasicCondition(IFilterTerm term)
    {
      var fieldName = GetColumnName(term.PropertyName);
      var clause = fieldName + " ";
      var paramName = GetParameterName();
      switch (term.Operator)
      {
        case FilterOperators.Equal when term.Value == null:
          clause += "IS NULL";
          break;
        case FilterOperators.NotEqual when term.Value == null:
          clause += "IS NOT NULL";
          break;
        case FilterOperators.Equal:
          clause += $"= {paramName}";
          break;
        case FilterOperators.NotEqual:
          clause += $"!= {paramName}";
          break;
        case FilterOperators.LessThan:
          clause += $"< {paramName}";
          break;
        case FilterOperators.LessThanOrEqual:
          clause += $"<= {paramName}";
          break;
        case FilterOperators.GreaterThan:
          clause += $"> {paramName}";
          break;
        case FilterOperators.GreaterThanOrEqual:
          clause += $">= {paramName}";
          break;
        case FilterOperators.StartsWith:
          clause += $"LIKE {paramName} + '%'";
          break;
        case FilterOperators.EndsWith:
          clause += $"LIKE '%' + {paramName}";
          break;
        case FilterOperators.Contains:
          clause += $"LIKE '%' + {paramName} + '%'";
          break;
      }

      return term.Value == null
        ? new Tuple<string, TDbParameter>(clause, default(TDbParameter))
        : new Tuple<string, TDbParameter>(clause, GetDbParameter(paramName, term.Value));
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

    private string GetParameterName()
    {
      return $"@p{_paramNo++}";
    }
  }
}

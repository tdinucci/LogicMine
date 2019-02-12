using System;
using System.Linq;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.Filter.Parse;
using Xunit;

namespace Test.LogicMine.DataObject.Filter.Parse
{
  public class FilterParserTest
  {
    [Fact]
    public void ArgException()
    {
      Assert.ThrowsAny<ArgumentException>(() => new FilterParser(null));
      Assert.ThrowsAny<ArgumentException>(() => new FilterParser(""));
      Assert.ThrowsAny<ArgumentException>(() => new FilterParser("  "));
    }

    [Fact]
    public void Simple()
    {
      const string expression = "field1 eq 100";
      var terms = new FilterParser(expression).Parse().Terms.ToArray();

      Assert.Single(terms);
      Assert.Equal("field1", terms[0].PropertyName);
      Assert.Equal(FilterOperators.Equal, terms[0].Operator);
      Assert.Equal("100", terms[0].Value);
    }

    [Fact]
    public void AndSimple()
    {
      const string expression = "field1 eq 100 and field2 startswith abc";
      var terms = new FilterParser(expression).Parse().Terms.ToArray();

      Assert.Equal(2, terms.Length);

      Assert.Equal("field1", terms[0].PropertyName);
      Assert.Equal(FilterOperators.Equal, terms[0].Operator);
      Assert.Equal("100", terms[0].Value);

      Assert.Equal("field2", terms[1].PropertyName);
      Assert.Equal(FilterOperators.StartsWith, terms[1].Operator);
      Assert.Equal("abc", terms[1].Value);
    }

    [Fact]
    public void AndComplex()
    {
      const string expression =
        "field1 eq 100 and field2 startswith abc AND field3 in (1,2,3) AnD field4 range 80..160 aNd hand le 4";
      var terms = new FilterParser(expression).Parse().Terms.ToArray();

      Assert.Equal(5, terms.Length);

      Assert.Equal("field1", terms[0].PropertyName);
      Assert.Equal(FilterOperators.Equal, terms[0].Operator);
      Assert.Equal("100", terms[0].Value);

      Assert.Equal("field2", terms[1].PropertyName);
      Assert.Equal(FilterOperators.StartsWith, terms[1].Operator);
      Assert.Equal("abc", terms[1].Value);

      var inTerm = terms[2] as InFilterTerm;
      Assert.Equal("field3", inTerm.PropertyName);
      Assert.Equal(FilterOperators.In, inTerm.Operator);
      Assert.Equal(new[] {"1", "2", "3"}, inTerm.Value);

      var rangeTerm = terms[3] as RangeFilterTerm;
      Assert.Equal("field4", rangeTerm.PropertyName);
      Assert.Equal(FilterOperators.Range, rangeTerm.Operator);
      Assert.Equal("80", rangeTerm.From);
      Assert.Equal("160", rangeTerm.To);

      Assert.Equal("hand", terms[4].PropertyName);
      Assert.Equal(FilterOperators.LessThanOrEqual, terms[4].Operator);
      Assert.Equal("4", terms[4].Value);
    }

    [Fact]
    public void AndComplexTyped()
    {
      const string expression =
        "intField eq 100 and stringField startswith abc AND decimalField in (1.12,2.23,3.34) AnD dateField range 2018-03-20..2019-04-5 aNd doubleField le 4.8";

      var terms = new FilterParser<FilterTermParserTest.FilteredType>(expression).Parse().Terms.ToArray();

      Assert.Equal(5, terms.Length);

      Assert.Equal(nameof(FilterTermParserTest.FilteredType.IntField), terms[0].PropertyName);
      Assert.Equal(FilterOperators.Equal, terms[0].Operator);
      Assert.Equal(100, terms[0].Value);

      Assert.Equal(nameof(FilterTermParserTest.FilteredType.StringField), terms[1].PropertyName);
      Assert.Equal(FilterOperators.StartsWith, terms[1].Operator);
      Assert.Equal("abc", terms[1].Value);

      var inTerm = terms[2] as InFilterTerm;
      Assert.Equal(nameof(FilterTermParserTest.FilteredType.DecimalField), inTerm.PropertyName);
      Assert.Equal(FilterOperators.In, inTerm.Operator);
      Assert.Contains(1.12m, inTerm.Value);
      Assert.Contains(2.23m, inTerm.Value);
      Assert.Contains(3.34m, inTerm.Value);

      var rangeTerm = terms[3] as RangeFilterTerm;
      Assert.Equal(nameof(FilterTermParserTest.FilteredType.DateField), rangeTerm.PropertyName);
      Assert.Equal(FilterOperators.Range, rangeTerm.Operator);
      Assert.Equal(new DateTime(2018, 3, 20), rangeTerm.From);
      Assert.Equal(new DateTime(2019, 4, 5), rangeTerm.To);

      Assert.Equal(nameof(FilterTermParserTest.FilteredType.DoubleField), terms[4].PropertyName);
      Assert.Equal(FilterOperators.LessThanOrEqual, terms[4].Operator);
      Assert.Equal(4.8, terms[4].Value);
    }

    public class FilteredType
    {
      public int? IntField { get; set; }
      public char CharField { get; set; }
      public string StringField { get; set; }
      public DateTime DateField { get; set; }
      public double DoubleField { get; set; }
      public decimal DecimalField { get; set; }
    }
  }
}

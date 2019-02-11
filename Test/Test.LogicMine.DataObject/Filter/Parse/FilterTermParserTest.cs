using System;
using System.Linq;
using FastMember;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.Filter.Parse;
using Xunit;

namespace Test.LogicMine.DataObject.Filter.Parse
{
  public class FilterTermParserTest
  {
    [Theory]
    [InlineData("field eq 100", "field", FilterOperators.Equal, "100")]
    [InlineData("field EQ 100", "field", FilterOperators.Equal, "100")]
    [InlineData("field eQ 100", "field", FilterOperators.Equal, "100")]

    [InlineData("field ne 100", "field", FilterOperators.NotEqual, "100")]
    [InlineData("field NE 100", "field", FilterOperators.NotEqual, "100")]
    [InlineData("field Ne 100", "field", FilterOperators.NotEqual, "100")]

    [InlineData("field lt 101", "field", FilterOperators.LessThan, "101")]
    [InlineData("field      lt      101", "field", FilterOperators.LessThan, "101")]
    [InlineData("field le 102", "field", FilterOperators.LessThanOrEqual, "102")]
    [InlineData("field gt 103", "field", FilterOperators.GreaterThan, "103")]
    [InlineData("field ge 104", "field", FilterOperators.GreaterThanOrEqual, "104")]

    [InlineData("field startswith abc", "field", FilterOperators.StartsWith, "abc")]
    [InlineData("field StArTsWiTh abc", "field", FilterOperators.StartsWith, "abc")]

    [InlineData("field contains lmn", "field", FilterOperators.Contains, "lmn")]
    [InlineData("field   conTains      lmn", "field", FilterOperators.Contains, "lmn")]

    [InlineData("field endswith xyz", "field", FilterOperators.EndsWith, "xyz")]
    [InlineData("field ENDSwith xyz", "field", FilterOperators.EndsWith, "xyz")]

    [InlineData(null, null, null, null, true)]
    [InlineData("", null, null, null, true)]
    [InlineData("   ", null, null, null, true)]

    [InlineData("field ? 100", null, null, null, false, true)]
    [InlineData("field", null, null, null, false, true)]

    [InlineData("field eq 2018-01-31", "field", FilterOperators.Equal, "2018-01-31")]
    public void Basic(string expression, string expectField, FilterOperators? expectedOperator, string expectedValue,
      bool expectArgEx = false, bool expectInvalidOpEx = false)
    {
      if (expectArgEx)
        Assert.ThrowsAny<ArgumentException>(() => new FilterTermParser(expression));
      else if (expectInvalidOpEx)
        Assert.ThrowsAny<InvalidOperationException>(() => new FilterTermParser(expression).Parse());
      else
      {
        var term = new FilterTermParser(expression).Parse();
        Assert.Equal(expectField, term.PropertyName);
        Assert.Equal(expectedOperator, term.Operator);
        Assert.Equal(expectedValue, term.Value);
      }
    }

    [Theory]
    [InlineData("intField eq 100", nameof(FilteredType.IntField), FilterOperators.Equal, 100)]
    [InlineData("intField EQ 100", nameof(FilteredType.IntField), FilterOperators.Equal, 100)]
    [InlineData("intField eQ 100", nameof(FilteredType.IntField), FilterOperators.Equal, 100)]

    [InlineData("intField ne 100", nameof(FilteredType.IntField), FilterOperators.NotEqual, 100)]
    [InlineData("intField NE 100", nameof(FilteredType.IntField), FilterOperators.NotEqual, 100)]
    [InlineData("intField Ne 100", nameof(FilteredType.IntField), FilterOperators.NotEqual, 100)]

    [InlineData("intField lt 101", nameof(FilteredType.IntField), FilterOperators.LessThan, 101)]
    [InlineData("intField      lt      101", nameof(FilteredType.IntField), FilterOperators.LessThan, 101)]
    [InlineData("intField le 102", nameof(FilteredType.IntField), FilterOperators.LessThanOrEqual, 102)]
    [InlineData("intField gt 103", nameof(FilteredType.IntField), FilterOperators.GreaterThan, 103)]
    [InlineData("intField ge 104", nameof(FilteredType.IntField), FilterOperators.GreaterThanOrEqual, 104)]

    [InlineData("doubleField eq 100.585", nameof(FilteredType.DoubleField), FilterOperators.Equal, 100.585)]

    [InlineData("stringField startswith abc", nameof(FilteredType.StringField), FilterOperators.StartsWith, "abc")]
    [InlineData("stringField StArTsWiTh abc", nameof(FilteredType.StringField), FilterOperators.StartsWith, "abc")]

    [InlineData("stringField contains lmn", nameof(FilteredType.StringField), FilterOperators.Contains, "lmn")]
    [InlineData("stringField   conTains      lmn", nameof(FilteredType.StringField), FilterOperators.Contains, "lmn")]

    [InlineData("stringField endswith xyz", nameof(FilteredType.StringField), FilterOperators.EndsWith, "xyz")]
    [InlineData("stringField ENDSwith xyz", nameof(FilteredType.StringField), FilterOperators.EndsWith, "xyz")]

    [InlineData("noField eq 100", null, null, null, false, true)]

    [InlineData(null, null, null, null, true)]
    [InlineData("", null, null, null, true)]
    [InlineData("   ", null, null, null, true)]

    [InlineData("intField ? 100", null, null, null, false, true)]
    [InlineData("intField", null, null, null, false, true)]
    public void BasicTyped(string expression, string expectField, FilterOperators? expectedOperator,
      object expectedValue, bool expectArgEx = false, bool expectInvalidOpEx = false)
    {
      var members = TypeAccessor.Create(typeof(FilteredType)).GetMembers()
        .ToDictionary(m => m.Name.ToLower(), m => m);

      if (expectArgEx)
      {
        Assert.ThrowsAny<ArgumentException>(() => new FilterTermParser<FilteredType>(expression, members));
      }
      else if (expectInvalidOpEx)
      {
        Assert.ThrowsAny<InvalidOperationException>(() =>
          new FilterTermParser<FilteredType>(expression, members).Parse());
      }
      else
      {
        var term = new FilterTermParser<FilteredType>(expression, members).Parse();
        Assert.Equal(expectField, term.PropertyName);
        Assert.Equal(expectedOperator, term.Operator);
        Assert.Equal(expectedValue, term.Value);
      }
    }

    [Theory]
    [InlineData("field range 1 .. 200", "field", FilterOperators.Range, "1", "200")]
    [InlineData("field RaNgE 8 .. 20", "field", FilterOperators.Range, "8", "20")]
    [InlineData("f   range    100..2000    ", "f", FilterOperators.Range, "100", "2000")]
    public void Range(string expression, string expectField, FilterOperators? expectOperator, string expectFrom,
      string expectTo)
    {
      var term = new FilterTermParser(expression).Parse() as RangeFilterTerm;
      Assert.NotNull(term);
      Assert.Equal(expectField, term.PropertyName);
      Assert.Equal(expectOperator, term.Operator);
      Assert.Equal(expectFrom, term.From);
      Assert.Equal(expectTo, term.To);
    }

    [Theory]
    [InlineData("intField range 1 .. 200", nameof(FilteredType.IntField), FilterOperators.Range, 1, 200)]
    [InlineData("doubleField RaNgE 8.9 .. 20.21", nameof(FilteredType.DoubleField), FilterOperators.Range, 8.9, 20.21)]
    [InlineData("intField   range    100..2000    ", nameof(FilteredType.IntField), FilterOperators.Range, 100, 2000)]
    public void RangeTyped(string expression, string expectField, FilterOperators? expectOperator, object expectFrom,
      object expectTo)
    {
      var members = TypeAccessor.Create(typeof(FilteredType)).GetMembers()
        .ToDictionary(m => m.Name.ToLower(), m => m);

      var term = new FilterTermParser<FilteredType>(expression, members).Parse() as RangeFilterTerm;
      Assert.NotNull(term);
      Assert.Equal(expectField, term.PropertyName);
      Assert.Equal(expectOperator, term.Operator);
      Assert.Equal(expectFrom, term.From);
      Assert.Equal(expectTo, term.To);
    }

    [Theory]
    [InlineData("field in (1,2,3,4)", "field", FilterOperators.In, new[] {"1", "2", "3", "4"})]
    [InlineData("field iN (1,2,3,4)", "field", FilterOperators.In, new[] {"1", "2", "3", "4"})]
    [InlineData("  f      in      (1,2,3,4)  ", "f", FilterOperators.In, new[] {"1", "2", "3", "4"})]
    [InlineData("field in (null,  1,2,3,4)", "field", FilterOperators.In, new[] {"null", "1", "2", "3", "4"})]
    public void In(string expression, string expectField, FilterOperators? expectOperator, string[] expectedValues)
    {
      var term = new FilterTermParser(expression).Parse() as InFilterTerm;
      Assert.NotNull(term);
      Assert.Equal(expectField, term.PropertyName);
      Assert.Equal(expectOperator, term.Operator);
      Assert.Equal(expectedValues, term.Value);
    }

    [Theory]
    [InlineData("intField in (1,2,3,4)", nameof(FilterTermParserTest.FilteredType.IntField), FilterOperators.In, new object[] {1, 2, 3, 4})]
    [InlineData("doubleField iN (1.2,2.3 ,3.4, 4.5)", nameof(FilterTermParserTest.FilteredType.DoubleField), FilterOperators.In,
      new object[] {1.2, 2.3, 3.4, 4.5})]
    [InlineData("  intField      in      (1,2,3,4)  ", nameof(FilterTermParserTest.FilteredType.IntField), FilterOperators.In, new object[] {1, 2, 3, 4})]
    [InlineData("charField in (a,b, c, d)", nameof(FilterTermParserTest.FilteredType.CharField), FilterOperators.In,
      new object[] {'a', 'b', 'c', 'd'})]
    public void InTyped(string expression, string expectField, FilterOperators? expectOperator, object[] expectedValues)
    {
      var members = TypeAccessor.Create(typeof(FilteredType)).GetMembers()
        .ToDictionary(m => m.Name.ToLower(), m => m);

      var term = new FilterTermParser<FilteredType>(expression, members).Parse() as InFilterTerm;
      Assert.NotNull(term);
      Assert.Equal(expectField, term.PropertyName);
      Assert.Equal(expectOperator, term.Operator);
      Assert.Equal(expectedValues, term.Value);
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

using System;
using System.Linq;
using FastMember;
using LogicMine.Api.Web.Filter;
using Xunit;

namespace Test.LogicMine.Api.Web.Filter
{
  public class RangeFilterTermParserTest
  {
    [Theory]
    [InlineData("fieldName", "1 .. 100", "1", "100")]
    [InlineData("fieldName", "1   ..    100", "1", "100")]
    [InlineData("fieldName", "1..100", "1", "100")]
    [InlineData("fieldName", "1.. 100", "1", "100")]
    [InlineData("fieldName", "1 ..100", "1", "100")]
    [InlineData("fieldName", "1   ..100", "1", "100")]
    [InlineData("fieldName", "1..   100", "1", "100")]
    [InlineData("field", "1234 .. 854225", "1234", "854225")]
    [InlineData("field", "a .. z", "a", "z")]
    [InlineData("field", "2018-01-01..2018-12-31", "2018-01-01", "2018-12-31")]

    [InlineData("", "1 .. 100", "1", "100", true)]
    [InlineData(" ", "1 .. 100", "1", "100", true)]
    [InlineData(null, "1 .. 100", "1", "100", true)]
    [InlineData("field", "", "1", "100", true)]
    [InlineData("field", " ", "1", "100", true)]
    [InlineData("field", null, "1", "100", true)]

    [InlineData("field", "10.1000", null, null, false, true)]
    [InlineData("field", "10", null, null, false, true)]
    public void General(string field, string rangeExpression, string expectFrom, string expectTo,
      bool expectArgEx = false, bool expectInvalidOpEx = false)
    {
      if (expectArgEx)
        Assert.ThrowsAny<ArgumentException>(() => new RangeFilterTermParser(field, rangeExpression));
      else if (expectInvalidOpEx)
        Assert.ThrowsAny<InvalidOperationException>(() => new RangeFilterTermParser(field, rangeExpression).Parse());
      else
      {
        var term = new RangeFilterTermParser(field, rangeExpression).Parse();
        Assert.Equal(field, term.PropertyName);
        Assert.Equal(expectFrom, term.From);
        Assert.Equal(expectTo, term.To);
      }
    }

    [Theory]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1 .. 100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1   ..    100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1..100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1.. 100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1 ..100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1   ..100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1..   100", 1, 100)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "1234 .. 854225", 1234, 854225)]
    [InlineData(nameof(FilterTermParserTest.FilteredType.CharField), "a .. z", 'a', 'z')]
    [InlineData(nameof(FilterTermParserTest.FilteredType.StringField), "abc .. xyz", "abc", "xyz")]

    [InlineData("", "1 .. 100", "1", "100", true)]
    [InlineData(" ", "1 .. 100", "1", "100", true)]
    [InlineData(null, "1 .. 100", "1", "100", true)]
    [InlineData("field", "", "1", "100", true)]
    [InlineData("field", " ", "1", "100", true)]
    [InlineData("field", null, "1", "100", true)]

    [InlineData("field", "10.1000", null, null, false, true)]
    [InlineData("field", "10", null, null, false, true)]
    public void GeneralTyped(string field, string rangeExpression, object expectFrom, object expectTo,
      bool expectArgEx = false, bool expectInvalidOpEx = false)
    {
      if (expectArgEx)
        Assert.ThrowsAny<ArgumentException>(() => new RangeFilterTermParser(field, rangeExpression));
      else if (expectInvalidOpEx)
        Assert.ThrowsAny<InvalidOperationException>(() => new RangeFilterTermParser(field, rangeExpression).Parse());
      else
      {
        var members = TypeAccessor.Create(typeof(FilteredRangeType)).GetMembers()
          .ToDictionary(m => m.Name.ToLower(), m => m);
        var term = new RangeFilterTermParser<FilteredRangeType>(field, rangeExpression, members).Parse();
        Assert.Equal(field, term.PropertyName);
        Assert.Equal(expectFrom, term.From);
        Assert.Equal(expectTo, term.To);
      }
    }

    [Fact]
    public void DateTimeTyped()
    {
      var members = TypeAccessor.Create(typeof(FilteredRangeType)).GetMembers()
        .ToDictionary(m => m.Name.ToLower(), m => m);
      var term = new RangeFilterTermParser<FilteredRangeType>("DateField", "2018-01-03..2018-06-25", members).Parse();
      Assert.Equal("DateField", term.PropertyName);
      Assert.Equal(new DateTime(2018, 1, 3), term.From);
      Assert.Equal(new DateTime(2018, 6, 25), term.To);
    }

    public class FilteredRangeType
    {
      public int IntField { get; set; }
      public char CharField { get; set; }
      public string StringField { get; set; }
      public DateTime DateField { get; set; }
    }
  }
}
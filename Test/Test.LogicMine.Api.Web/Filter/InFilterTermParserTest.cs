using System;
using System.Linq;
using FastMember;
using LogicMine.Api.Web.Filter;
using Xunit;

namespace Test.LogicMine.Api.Web.Filter
{
  public class InFilterTermParserTest
  {
    [Theory]
    [InlineData("fieldName", "(1,2,3,4)", new[] {"1", "2", "3", "4"})]
    [InlineData("fieldName", "(null,1,2,3,4)", new[] {"null", "1", "2", "3", "4"})]
    [InlineData("field", "(null,   1   ,   2,    3,4)", new[] {"null", "1", "2", "3", "4"})]
    [InlineData("field", "(null,   one   ,   two,    three,four)", new[] {"null", "one", "two", "three", "four"})]
    [InlineData("field", "(   one piggy   ,   two piggies,    three piggies,four piggies)",
      new[] {"one piggy", "two piggies", "three piggies", "four piggies"})]
    [InlineData("field", "(2018-01-01,   2018-01-02,   2018-01-03,    2018-01-04)",
      new[] {"2018-01-01", "2018-01-02", "2018-01-03", "2018-01-04"})]

    [InlineData("", "(1,2)", null, true)]
    [InlineData("   ", "(1,2)", null, true)]
    [InlineData(null, "(1,2)", null, true)]

    [InlineData("field", "", null, true)]
    [InlineData("field", "   ", null, true)]
    [InlineData("field", null, null, true)]

    [InlineData("field", "()", null, false, true)]
    [InlineData("field", "1,2,3", null, false, true)]
    [InlineData("field", "(1,2,3", null, false, true)]
    [InlineData("field", "1,2,3)", null, false, true)]
    public void General(string field, string inExpression, string[] expected, bool expectArgEx = false,
      bool expectInvalidOpEx = false)
    {
      if (expectArgEx)
        Assert.ThrowsAny<ArgumentException>(() => new InFilterTermParser(field, inExpression));
      else if (expectInvalidOpEx)
        Assert.ThrowsAny<InvalidOperationException>(() => new InFilterTermParser(field, inExpression).Parse());
      else
      {
        var term = new InFilterTermParser(field, inExpression).Parse();
        Assert.Equal(field, term.PropertyName);
        Assert.Equal(expected, term.Value);
      }
    }

    [Theory]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "(1,2,3,4)", new object[] {1, 2, 3, 4})]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "(null,1,2,3,4)", new object[] {null, 1, 2, 3, 4})]
    [InlineData(nameof(FilterTermParserTest.FilteredType.IntField), "(null,   1   ,   2,    3,4)", new object[] {null, 1, 2, 3, 4})]
    [InlineData(nameof(FilterTermParserTest.FilteredType.StringField), "(null,   one   ,   two,    three,four)", new[] {null, "one", "two", "three", "four"})]
    [InlineData(nameof(FilterTermParserTest.FilteredType.StringField), "(   one piggy   ,   two piggies,    three piggies,four piggies)",
      new[] {"one piggy", "two piggies", "three piggies", "four piggies"})]

    [InlineData("", "(1,2)", null, true)]
    [InlineData("   ", "(1,2)", null, true)]
    [InlineData(null, "(1,2)", null, true)]

    [InlineData("field", "", null, true)]
    [InlineData("field", "   ", null, true)]
    [InlineData("field", null, null, true)]

    [InlineData("field", "()", null, false, true)]
    [InlineData("field", "1,2,3", null, false, true)]
    [InlineData("field", "(1,2,3", null, false, true)]
    [InlineData("field", "1,2,3)", null, false, true)]
    public void GeneralTyped(string field, string inExpression, object[] expected, bool expectArgEx = false,
      bool expectInvalidOpEx = false)
    {
      if (expectArgEx)
        Assert.ThrowsAny<ArgumentException>(() => new InFilterTermParser(field, inExpression));
      else if (expectInvalidOpEx)
        Assert.ThrowsAny<InvalidOperationException>(() => new InFilterTermParser(field, inExpression).Parse());
      else
      {
        var members = TypeAccessor.Create(typeof(FilteredInType)).GetMembers()
          .ToDictionary(m => m.Name.ToLower(), m => m);

        var term = new InFilterTermParser<FilteredInType>(field.ToUpper(), inExpression, members).Parse();
        Assert.Equal(field, term.PropertyName);
        Assert.Equal(expected, term.Value);
      }
    }

    [Fact]
    public void DateTimeTyped()
    {
      var members = TypeAccessor.Create(typeof(FilteredInType)).GetMembers().ToDictionary(m => m.Name.ToLower(), m => m);
      var term = new InFilterTermParser<FilteredInType>("DateField", "(2018-01-03, 2018-06-25, 2018-08-6, 2017-10-30)", members).Parse();
      Assert.Equal("DateField", term.PropertyName);

      var dates = term.Value.ToArray();
      Assert.Equal(new DateTime(2018, 1, 3), dates[0]);
      Assert.Equal(new DateTime(2018, 6, 25), dates[1]);
      Assert.Equal(new DateTime(2018, 8, 6), dates[2]);
      Assert.Equal(new DateTime(2017, 10, 30), dates[3]);
    }

    public class FilteredInType
    {
      public int? IntField { get; set; }
      public char CharField { get; set; }
      public string StringField { get; set; }
      public DateTime DateField { get; set; }
    }
  }
}
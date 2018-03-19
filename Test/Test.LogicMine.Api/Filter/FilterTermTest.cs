using System;
using System.Linq;
using LogicMine.Api.Filter;
using Xunit;

namespace Test.LogicMine.Api.Filter
{
  public class FilterTermTest
  {
    [Fact]
    public void ConstructBasicFilterTerm()
    {
      var name = Guid.NewGuid().ToString();
      var op = FilterOperators.NotEqual;
      var value = Guid.NewGuid();
      var term = new FilterTerm(name, op, value);

      Assert.Equal(name, term.PropertyName);
      Assert.Equal(op, term.Operator);
      Assert.Equal(value, term.Value);
    }

    [Fact]
    public void ConstructInFilterTerm()
    {
      var name = Guid.NewGuid().ToString();
      var values = new object[] {2, 4, 6, 8, 10};
      var term = new InFilterTerm(name, values.ToArray());

      Assert.Equal(name, term.PropertyName);
      Assert.Equal(FilterOperators.In, term.Operator);
      Assert.Equal(values, term.Value);
    }

    [Fact]
    public void ConstructRangeFilterTerm()
    {
      var name = Guid.NewGuid().ToString();
      var from = 99;
      var to = 6563;
      var term = new RangeFilterTerm(name, from, to);

      Assert.Equal(name, term.PropertyName);
      Assert.Equal(FilterOperators.Range, term.Operator);
      Assert.Equal(from, term.From);
      Assert.Equal(to, term.To);
    }
  }
}

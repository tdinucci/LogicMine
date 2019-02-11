using System;
using System.Linq;
using LogicMine.DataObject.Filter;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject.Filter
{
  public class FilterTest
  {
    [Fact]
    public void Construct()
    {
      var filter = new global::LogicMine.DataObject.Filter.Filter(new[]
      {
        new FilterTerm("one", FilterOperators.LessThan, 88),
        new RangeFilterTerm("two", 6, 25),
        new InFilterTerm("three", new object[] {1, 2, 3})
      });

      Assert.Equal(3, filter.Terms.Count());

      Assert.Contains(filter.Terms,
        p => p.PropertyName == "one" && p.Operator == FilterOperators.LessThan && (int) p.Value == 88);

      Assert.Contains(filter.Terms,
        p => p.PropertyName == "two" && p.Operator == FilterOperators.Range
                                     && (int) ((RangeFilterTerm) p).From == 6 &&
                                     (int) ((RangeFilterTerm) p).To == 25);
      Assert.Contains(filter.Terms,
        p => p.PropertyName == "three" && p.Operator == FilterOperators.In
                                       && ((InFilterTerm) p).Value.ToArray().Length == 3);
    }

    [Fact]
    public void ConstructGeneric()
    {
      var filter = new Filter<Frog>(new[]
      {
        new FilterTerm(nameof(Frog.Id), FilterOperators.LessThan, 88),
        new InFilterTerm(nameof(Frog.Name), new object[] {"kermit", "frank", "freddy"}),
        new RangeFilterTerm(nameof(Frog.DateOfBirth), DateTime.Today.AddDays(-7), DateTime.Today.AddDays(8))
      });
      Assert.Equal(3, filter.Terms.Count());

      Assert.Contains(filter.Terms,
        p => p.PropertyName == nameof(Frog.Id) && p.Operator == FilterOperators.LessThan && (int) p.Value == 88);

      Assert.Contains(filter.Terms,
        p => p.PropertyName == nameof(Frog.Name) && p.Operator == FilterOperators.In
                                                 && ((InFilterTerm) p).Value.ToArray().Length == 3);

      Assert.Contains(filter.Terms,
        p => p.PropertyName == nameof(Frog.DateOfBirth) && p.Operator == FilterOperators.Range
                                                        && (DateTime) ((RangeFilterTerm) p).From ==
                                                        DateTime.Today.AddDays(-7) &&
                                                        (DateTime) ((RangeFilterTerm) p).To ==
                                                        DateTime.Today.AddDays(8));
    }

    [Fact]
    public void AddTerm()
    {
      var filter =
        new global::LogicMine.DataObject.Filter.Filter(new[]
          {new FilterTerm("str", FilterOperators.StartsWith, "hello")});
      Assert.True(filter.Terms.Count() == 1);

      filter.AddTerm(new FilterTerm("one1", FilterOperators.GreaterThanOrEqual, 75));
      filter.AddTerm(new RangeFilterTerm("two2", 66, 250));
      filter.AddTerm(new InFilterTerm("three3", new object[] {1, 2, 3, 4, 5}));

      Assert.Equal(4, filter.Terms.Count());

      Assert.Contains(filter.Terms,
        p => p.PropertyName == "str" && p.Operator == FilterOperators.StartsWith && (string) p.Value == "hello");

      Assert.Contains(filter.Terms,
        p => p.PropertyName == "one1" && p.Operator == FilterOperators.GreaterThanOrEqual && (int) p.Value == 75);

      Assert.Contains(filter.Terms,
        p => p.PropertyName == "two2" && p.Operator == FilterOperators.Range
                                      && (int) ((RangeFilterTerm) p).From == 66 &&
                                      (int) ((RangeFilterTerm) p).To == 250);

      Assert.Contains(filter.Terms,
        p => p.PropertyName == "three3" && p.Operator == FilterOperators.In
                                        && ((InFilterTerm) p).Value.ToArray().Length == 5);
    }

    [Fact]
    public void Convert()
    {
      var frogFilter = new Filter<Frog>(new[]
      {
        new FilterTerm(nameof(Frog.Id), FilterOperators.LessThan, 88),
        new InFilterTerm(nameof(Frog.Name), new object[] {"kermit", "frank", "freddy"}),
        new RangeFilterTerm(nameof(Frog.DateOfBirth), DateTime.Today.AddDays(-7), DateTime.Today.AddDays(8))
      });

      var altFrogFilter = frogFilter.Convert<AltFrog>((pn) =>
      {
        if (pn == nameof(Frog.Id))
          return nameof(AltFrog.FrogId);
        if (pn == nameof(Frog.Name))
          return nameof(AltFrog.FrogName);
        if (pn == nameof(Frog.DateOfBirth))
          return nameof(AltFrog.FrogDateOfBirth);

        return pn;
      });

      Assert.Equal(3, altFrogFilter.Terms.Count());

      Assert.Contains(altFrogFilter.Terms,
        p => p.PropertyName == nameof(AltFrog.FrogId) && p.Operator == FilterOperators.LessThan && (int) p.Value == 88);

      Assert.Contains(altFrogFilter.Terms,
        p => p.PropertyName == nameof(AltFrog.FrogName) && p.Operator == FilterOperators.In
                                                        && ((InFilterTerm) p).Value.ToArray().Length == 3);

      Assert.Contains(altFrogFilter.Terms,
        p => p.PropertyName == nameof(AltFrog.FrogDateOfBirth) && p.Operator == FilterOperators.Range
                                                               && (DateTime) ((RangeFilterTerm) p).From ==
                                                               DateTime.Today.AddDays(-7) &&
                                                               (DateTime) ((RangeFilterTerm) p).To ==
                                                               DateTime.Today.AddDays(8));
    }
  }
}
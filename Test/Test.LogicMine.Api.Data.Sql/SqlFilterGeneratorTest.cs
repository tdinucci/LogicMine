using LogicMine.Api.Data.Sql;
using LogicMine.Api.Filter;
using System;
using Xunit;

namespace Test.LogicMine.Api.Data.Sql
{
  public class SqlFilterGeneratorTest
  {
    [Fact]
    public void General()
    {
      var filter = new Filter(new[]
      {
        new FilterTerm("A", FilterOperators.Equal, "B"),
        new FilterTerm("B", FilterOperators.LessThan, 8),
        new FilterTerm("C", FilterOperators.GreaterThan, DateTime.Today),
        new InFilterTerm("D", new object[]{1, 2, 3}),
        new RangeFilterTerm("E", DateTime.Today, DateTime.Today.AddDays(7))
      });
      var generator = new SqlFilterGenerator(filter);
      var dbFilter = generator.Generate();

      Assert.Equal("WHERE A = @p0 AND B < @p1 AND C > @p2 AND D IN (@p3,@p4,@p5) AND E BETWEEN @p6 AND @p7",
        dbFilter.WhereClause);

      Assert.Equal(8, dbFilter.Parameters.Length);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p0" && (string) p.Value == "B");
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p1" && (int)p.Value == 8);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p2" && (DateTime)p.Value == DateTime.Today);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p3" && (int)p.Value == 1);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p4" && (int)p.Value == 2);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p5" && (int)p.Value == 3);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p6" && (DateTime)p.Value == DateTime.Today);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p7" && (DateTime)p.Value == DateTime.Today.AddDays(7));
    }

    [Fact]
    public void Map()
    {
      var filter = new Filter(new[]
      {
        new FilterTerm("A", FilterOperators.Equal, "B"),
        new FilterTerm("B", FilterOperators.LessThan, 8),
        new FilterTerm("C", FilterOperators.GreaterThan, DateTime.Today),
        new InFilterTerm("D", new object[]{1, 2, 3}),
        new RangeFilterTerm("E", DateTime.Today, DateTime.Today.AddDays(7))
      });
      var generator = new SqlFilterGenerator(filter, (pn) => pn + "x");
      var dbFilter = generator.Generate();

      Assert.Equal("WHERE Ax = @p0 AND Bx < @p1 AND Cx > @p2 AND Dx IN (@p3,@p4,@p5) AND Ex BETWEEN @p6 AND @p7",
        dbFilter.WhereClause);

      Assert.Equal(8, dbFilter.Parameters.Length);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p0" && (string)p.Value == "B");
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p1" && (int)p.Value == 8);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p2" && (DateTime)p.Value == DateTime.Today);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p3" && (int)p.Value == 1);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p4" && (int)p.Value == 2);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p5" && (int)p.Value == 3);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p6" && (DateTime)p.Value == DateTime.Today);
      Assert.Contains(dbFilter.Parameters, p => p.ParameterName == "@p7" && (DateTime)p.Value == DateTime.Today.AddDays(7));
    }
  }
}

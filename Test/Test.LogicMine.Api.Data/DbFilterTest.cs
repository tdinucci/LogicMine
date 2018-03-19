using LogicMine.Api.Data;
using System.Data.SqlClient;
using Xunit;

namespace Test.LogicMine.Api.Data
{
  public class DbFilterTest
  {
    [Fact]
    public void Construct()
    {
      var filter = new DbFilter<SqlParameter>("WHERE a = @a and b = @b",
        new[] {new SqlParameter("@a", 1), new SqlParameter("@b", 2)});

      Assert.Equal("WHERE a = @a and b = @b", filter.WhereClause);
      Assert.Equal(2, filter.Parameters.Length);
      Assert.Contains(filter.Parameters, p => p.ParameterName == "@a" && (int)p.Value == 1);
      Assert.Contains(filter.Parameters, p => p.ParameterName == "@b" && (int)p.Value == 2);
    }
  }
}

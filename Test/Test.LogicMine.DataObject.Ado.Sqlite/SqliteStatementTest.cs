using System.Data;
using LogicMine.DataObject.Ado.Sqlite;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Test.LogicMine.DataObject.Ado.Sqlite
{
  public class SqliteStatementTest
  {
    [Fact]
    public void ConstructBasic()
    {
      var statement = new SqliteStatement("select * from frog");
      Assert.Equal("select * from frog", statement.Text);
      Assert.Empty(statement.Parameters);
      Assert.Equal(CommandType.Text, statement.Type);
    }

    [Fact]
    public void ConstructType()
    {
      var statement = new SqliteStatement("exec proc", CommandType.StoredProcedure);
      Assert.Equal("exec proc", statement.Text);
      Assert.Empty(statement.Parameters);
      Assert.Equal(CommandType.StoredProcedure, statement.Type);
    }

    [Fact]
    public void ConstructParameters()
    {
      var statement = new SqliteStatement(
        "select * from frog where species like @species and colour like @colour",
        new SqliteParameter("@species", "tree"), new SqliteParameter("@colour", "green"));

      Assert.Equal("select * from frog where species like @species and colour like @colour", statement.Text);
      Assert.Equal(CommandType.Text, statement.Type);
      Assert.Equal(2, statement.Parameters.Length);
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@species" && (string) p.Value == "tree");
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@colour" && (string) p.Value == "green");
    }

    [Fact]
    public void ConstructTypeParameters()
    {
      var statement = new SqliteStatement(
        "exec getfrogs @species, @colour", CommandType.StoredProcedure,
        new SqliteParameter("@species", "tree"), new SqliteParameter("@colour", "green"));

      Assert.Equal("exec getfrogs @species, @colour", statement.Text);
      Assert.Equal(CommandType.StoredProcedure, statement.Type);
      Assert.Equal(2, statement.Parameters.Length);
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@species" && (string) p.Value == "tree");
      Assert.Contains(statement.Parameters, p => p.ParameterName == "@colour" && (string) p.Value == "green");
    }
  }
}

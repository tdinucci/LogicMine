﻿using System.Data;
using LogicMine.DataObject.Ado.PostgreSql;
using Npgsql;
using Xunit;

namespace Test.LogicMine.DataObject.Ado.PostgreSql
{
    public class PostgreSqlStatementTest
    {
        [Fact]
        public void ConstructBasic()
        {
            var statement = new PostgreSqlStatement("select * from frog");
            Assert.Equal("select * from frog", statement.Text);
            Assert.Empty(statement.Parameters);
            Assert.Equal(CommandType.Text, statement.Type);
        }

        [Fact]
        public void ConstructType()
        {
            var statement = new PostgreSqlStatement("exec proc", CommandType.StoredProcedure);
            Assert.Equal("exec proc", statement.Text);
            Assert.Empty(statement.Parameters);
            Assert.Equal(CommandType.StoredProcedure, statement.Type);
        }

        [Fact]
        public void ConstructParameters()
        {
            var statement = new PostgreSqlStatement(
                "select * from frog where species like @species and colour like @colour",
                new NpgsqlParameter("@species", "tree"), new NpgsqlParameter("@colour", "green"));

            Assert.Equal("select * from frog where species like @species and colour like @colour", statement.Text);
            Assert.Equal(CommandType.Text, statement.Type);
            Assert.Equal(2, statement.Parameters.Length);
            Assert.Contains(statement.Parameters, p => p.ParameterName == "@species" && (string) p.Value == "tree");
            Assert.Contains(statement.Parameters, p => p.ParameterName == "@colour" && (string) p.Value == "green");
        }

        [Fact]
        public void ConstructTypeParameters()
        {
            var statement = new PostgreSqlStatement(
                "exec getfrogs @species, @colour", CommandType.StoredProcedure,
                new NpgsqlParameter("@species", "tree"), new NpgsqlParameter("@colour", "green"));

            Assert.Equal("exec getfrogs @species, @colour", statement.Text);
            Assert.Equal(CommandType.StoredProcedure, statement.Type);
            Assert.Equal(2, statement.Parameters.Length);
            Assert.Contains(statement.Parameters, p => p.ParameterName == "@species" && (string) p.Value == "tree");
            Assert.Contains(statement.Parameters, p => p.ParameterName == "@colour" && (string) p.Value == "green");
        }
    }
}
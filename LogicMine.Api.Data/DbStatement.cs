/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Data;

namespace LogicMine.Api.Data
{
  /// <summary>
  /// Represents a database statement
  /// </summary>
  /// <typeparam name="TDbParameter">The parameter type to use with the chosen database connection</typeparam>
  public class DbStatement<TDbParameter> : IDbStatement<TDbParameter> where TDbParameter : IDbDataParameter
  {
    /// <inheritdoc />
    public string Text { get; }

    /// <inheritdoc />
    public CommandType Type { get; }

    /// <inheritdoc />
    public TDbParameter[] Parameters { get; }

    /// <summary>
    /// Construct a new DbStatement
    /// </summary>
    /// <param name="text">The statement text, i.e. SQL</param>
    /// <param name="type">The type of statement</param>
    /// <param name="parameters">The parameters which participate in the statement</param>
    public DbStatement(string text, CommandType type, params TDbParameter[] parameters)
    {
      if (string.IsNullOrWhiteSpace(text))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

      Text = text;
      Type = type;
      Parameters = parameters ?? new TDbParameter[0];
    }

    /// <summary>
    /// Construct a new DbStatement
    /// </summary>
    /// <param name="text">The statement text, i.e. SQL</param>
    /// <param name="type">The type of statement</param>
    public DbStatement(string text, CommandType type) : this(text, type, null)
    {
    }

    /// <summary>
    /// Construct a new DbStatement which has the default Type of CommandType.Text
    /// </summary>
    /// <param name="text">The statement text, i.e. SQL</param>
    /// <param name="parameters">The parameters which participate in the statement</param>
    public DbStatement(string text, params TDbParameter[] parameters) : this(text, CommandType.Text, parameters)
    {
    }

    /// <summary>
    /// Construct a new DbStatement which has the default Type of CommandType.Text
    /// </summary>
    /// <param name="text">The statement text, i.e. SQL</param>
    public DbStatement(string text) : this(text, CommandType.Text, null)
    {
    }
  }
}
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
using Microsoft.Data.Sqlite;

namespace LogicMine.Api.Data.Sqlite
{
  /// <summary>
  /// A terminal layer for dealing with T's which are stored in an SQLite database
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the terminals operate on</typeparam>
  public abstract class SqliteLayer<TId, T> : DbLayer<TId, T, SqliteParameter>
    where T : new()
  {
    /// <summary>
    /// Construct a new SqliteDbLayer
    /// </summary>
    /// <param name="connectionString">The db's connection string</param>
    /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
    /// <param name="mapper">An object-relational mapper</param>
    protected SqliteLayer(string connectionString, SqliteObjectDescriptor<T> descriptor, IDbMapper<T> mapper) :
      base(new SqliteInterface(connectionString), descriptor, mapper)
    {
    }
  }
}

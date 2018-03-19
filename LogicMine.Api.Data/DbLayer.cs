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
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;

namespace LogicMine.Api.Data
{
  /// <summary>
  /// A terminal layer for dealing with T's which are stored in a database
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the terminals operate on</typeparam>
  /// <typeparam name="TDbParameter">The type of parameter used with the database</typeparam>
  public abstract class DbLayer<TId, T, TDbParameter> : IDbLayer<TId,T>
    where T : new()
    where TDbParameter : IDbDataParameter
  {
    /// <summary>
    /// Typically, if an identifier is given a reserved word for a name then it must be formatted 
    /// in a particular way, e.g. [Group]
    /// </summary>
    protected virtual string SafeIdentifierFormat => "[{0}]";

    /// <summary>
    /// An interface to an underlying database
    /// </summary>
    protected IDbInterface<TDbParameter> DbInterface { get; }

    /// <summary>
    /// Contains metadata to enable mapping T's to database tables
    /// </summary>
    public IDbObjectDescriptor<T> Descriptor { get; }

    /// <summary>
    /// An object-relational mapper
    /// </summary>
    protected IDbMapper<T> Mapper { get; }

    /// <summary>
    /// Construct a new DbLayer
    /// </summary>
    /// <param name="dbInterface">An interface to an underlying database</param>
    /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
    /// <param name="mapper">An object-relational mapper</param>
    protected DbLayer(IDbInterface<TDbParameter> dbInterface, IDbObjectDescriptor<T> descriptor, IDbMapper<T> mapper)
    {
      DbInterface = dbInterface ?? throw new ArgumentNullException(nameof(dbInterface));
      Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
      Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Get a statement to select a single record
    /// </summary>
    /// <param name="identity">The identity of the T to select</param>
    /// <returns>A statement to represent the "select" operation</returns>
    protected abstract IDbStatement<TDbParameter> GetSelectDbStatement(TId identity);

    /// <summary>
    /// Get a statement to select all records of type T which match the request
    /// </summary>
    /// <param name="request">The "get collection" request</param>
    /// <returns>A statement to represent the "select" operation</returns>
    protected abstract IDbStatement<TDbParameter> GetSelectDbStatement(IGetCollectionRequest<T> request);

    /// <summary>
    /// Get a statement to insert an object into the underlying database.
    /// This statement should also return the identity of the inserted record.
    /// </summary>
    /// <param name="obj">The T to insert</param>
    /// <returns>A statement to represent the "insert" operation</returns>
    protected abstract IDbStatement<TDbParameter> GetInsertDbStatement(T obj);

    /// <summary>
    /// Get a statement to perform a partial update of a record
    /// </summary>
    /// <param name="request">The partial update request</param>
    /// <returns>A statement to represent the "update" operation</returns>
    protected abstract IDbStatement<TDbParameter> GetUpdateDbStatement(IPatchRequest<TId, T> request);

    /// <summary>
    /// Get a statement to delete a single T
    /// </summary>
    /// <param name="identity">The identity of the T to delete</param>
    /// <returns>A statement to represent the "delete" operation</returns>
    protected abstract IDbStatement<TDbParameter> GetDeleteDbStatement(TId identity);

    /// <summary>
    /// Get a statement to delete all T's which match the request
    /// </summary>
    /// <param name="request">The request to delete a collection of objects</param>
    /// <returns>A statement to represent the "delete" operation</returns>
    protected abstract IDbStatement<TDbParameter> GetDeleteDbStatement(IDeleteCollectionRequest<T> request);

    /// <summary>
    /// Ensures that a column identifier is safe, i.e. if the identifier is a reserved DB keyword 
    /// then it will need to be escaped before it is safe.
    /// </summary>
    /// <param name="columnName"></param>
    /// <returns></returns>
    protected virtual string MakeColumnNameSafe(string columnName)
    {
      if (string.IsNullOrWhiteSpace(SafeIdentifierFormat))
        return columnName;

      if (!string.IsNullOrWhiteSpace(columnName))
      {
        columnName = columnName.Trim();
        if (!columnName.StartsWith(SafeIdentifierFormat[0].ToString()))
          columnName = string.Format(SafeIdentifierFormat, columnName);
      }

      return columnName;
    }

    /// <summary>
    /// Retreives the requested T from the database and adds it to the baskets AscendPayload
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public async Task AddResultAsync(IGetBasket<TId, T> basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      using (var rdr = await DbInterface.GetReaderAsync(GetSelectDbStatement(basket.DescentPayload))
        .ConfigureAwait(false))
      {
        if (!await rdr.ReadAsync().ConfigureAwait(false))
          throw new InvalidOperationException($"No '{typeof(T)}' record found");

        basket.AscentPayload = Mapper.MapObject(rdr);
      }
    }

    /// <summary>
    /// Retrieves the requested collection of T's from the database and adds them to the baskets AscendPayload
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public async Task AddResultAsync(IGetCollectionBasket<T> basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      using (var rdr = await DbInterface.GetReaderAsync(GetSelectDbStatement(basket.DescentPayload))
        .ConfigureAwait(false))
      {
        var objs = new List<T>();
        while (await rdr.ReadAsync().ConfigureAwait(false))
        {
          var obj = Mapper.MapObject(rdr);
          objs.Add(obj);
        }

        basket.AscentPayload = objs.ToArray();
      }
    }

    /// <summary>
    /// Inserts the requested T into the database and sets the baskets AscentPayload to new records identity
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public async Task AddResultAsync(IPostBasket<T, TId> basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      var id = await DbInterface.ExecuteScalarAsync(GetInsertDbStatement(basket.DescentPayload))
        .ConfigureAwait(false);

      basket.AscentPayload = Descriptor.ProjectColumnValue<TId>(id);
    }

    /// <summary>
    /// Performs a partial update to a T in the database and sets the baskets AscentPayload to the number of records affected
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public async Task AddResultAsync(IPatchBasket<TId, T, int> basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      basket.AscentPayload = await DbInterface.ExecuteNonQueryAsync(GetUpdateDbStatement(basket.DescentPayload))
        .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a T from the database and sets the baskets AscentPayload to the number of records affected
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public async Task AddResultAsync(IDeleteBasket<TId, int> basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      basket.AscentPayload = await DbInterface.ExecuteNonQueryAsync(GetDeleteDbStatement(basket.DescentPayload))
        .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a collection of T's from the database and sets the baskets AscentPayload to the number of records affected
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public async Task AddResultAsync(IDeleteCollectionBasket<T, int> basket)
    {
      if (basket == null)
        throw new ArgumentNullException(nameof(basket));

      basket.AscentPayload = await DbInterface.ExecuteNonQueryAsync(GetDeleteDbStatement(basket.DescentPayload))
        .ConfigureAwait(false);
    }
  }
}

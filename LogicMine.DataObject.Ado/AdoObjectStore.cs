using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// An object store for T's which are stored in a database that has an ADO.Net data provider
    /// </summary>
    /// <typeparam name="T">The type which the store operates on</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    /// <typeparam name="TDbParameter">The type of parameter used with the ADO.Net data provider</typeparam>
    public abstract class AdoObjectStore<T, TId, TDbParameter> : IDataObjectStore<T, TId>
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
        public IDataObjectDescriptor Descriptor { get; }

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
        protected AdoObjectStore(IDbInterface<TDbParameter> dbInterface, IDataObjectDescriptor descriptor,
            IDbMapper<T> mapper)
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
        /// Get a statement to select all records of type T which match the request.
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        /// <param name="max">The maximum number of records desired</param>
        /// <param name="page">The page within the results that is desired</param>
        /// <returns>A statement to represent the "select" operation</returns>
        protected abstract IDbStatement<TDbParameter> GetSelectDbStatement(IFilter<T> filter, int? max = null,
            int? page = null);

        /// <summary>
        /// Get a statement to insert an object into the underlying database.
        /// This statement should also return the identity of the inserted record.
        /// </summary>
        /// <param name="obj">The T to insert</param>
        /// <returns>A statement to represent the "insert" operation</returns>
        protected abstract IDbStatement<TDbParameter> GetInsertDbStatement(T obj);

        /// <summary>
        /// Get a statement to insert a collection of objects into the underlying database.
        /// </summary>
        /// <param name="objs">The T's to insert</param>
        /// <returns>A statement to represent the "insert" operation</returns>
        protected abstract IDbStatement<TDbParameter> GetInsertDbStatement(IEnumerable<T> objs);
        
        /// <summary>
        /// Get a statement to perform an update of a record
        /// </summary>
        /// <param name="identity">The identity of the T to update</param>
        /// <param name="modifiedProperties">The properties to change on the T</param>
        /// <returns>A statement to represent the "update" operation</returns>
        protected abstract IDbStatement<TDbParameter> GetUpdateDbStatement(TId identity,
            IDictionary<string, object> modifiedProperties);

        /// <summary>
        /// Get a statement to delete a single T
        /// </summary>
        /// <param name="identity">The identity of the T to delete</param>
        /// <returns>A statement to represent the "delete" operation</returns>
        protected abstract IDbStatement<TDbParameter> GetDeleteDbStatement(TId identity);

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

        /// <inheritdoc />
        public Task<T[]> GetCollectionAsync(int? max = null, int? page = null)
        {
            return GetCollectionAsync(null, max, page);
        }

        /// <inheritdoc />
        public async Task<T[]> GetCollectionAsync(IFilter<T> filter, int? max = null, int? page = null)
        {
            var statement = GetSelectDbStatement(filter, max, page);
            using (var rdr = await DbInterface.GetReaderAsync(statement).ConfigureAwait(false))
            {
                var objs = new List<T>();
                while (await rdr.ReadAsync().ConfigureAwait(false))
                {
                    var obj = Mapper.MapObject(rdr);
                    objs.Add(obj);
                }

                return objs.ToArray();
            }
        }

        /// <inheritdoc />
        public async Task<T> GetByIdAsync(TId id)
        {
            var statement = GetSelectDbStatement(id);
            using (var rdr = await DbInterface.GetReaderAsync(statement).ConfigureAwait(false))
            {
                if (!await rdr.ReadAsync().ConfigureAwait(false))
                    throw new InvalidOperationException($"No '{typeof(T)}' record found");

                return Mapper.MapObject(rdr);
            }
        }

        /// <inheritdoc />
        public async Task<TId> CreateAsync(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var statement = GetInsertDbStatement(obj);
            var id = await DbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false);

            return Descriptor.ProjectColumnValue<TId>(id);
        }

        /// <inheritdoc />
        public async Task CreateCollectionAsync(IEnumerable<T> objs)
        {
            if (objs == null) throw new ArgumentNullException(nameof(objs));

            var statement = GetInsertDbStatement(objs);
            var recordsAffected = await DbInterface.ExecuteNonQueryAsync(statement).ConfigureAwait(false);
            if (recordsAffected == 0)
            {
                throw new InvalidOperationException(
                    "Creation of collection failed, expected to insert at least 1 record but inserted none");
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TId id, IDictionary<string, object> modifiedProperties)
        {
            if (modifiedProperties == null) throw new ArgumentNullException(nameof(modifiedProperties));

            var statement = GetUpdateDbStatement(id, modifiedProperties);

            var recordsAffected = await DbInterface.ExecuteNonQueryAsync(statement).ConfigureAwait(false);
            if (recordsAffected != 1)
            {
                throw new InvalidOperationException(
                    $"Update failed, expected to update 1 record but updated {recordsAffected}");
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TId id)
        {
            var statement = GetDeleteDbStatement(id);
            var recordsAffected = await DbInterface.ExecuteNonQueryAsync(statement).ConfigureAwait(false);
            if (recordsAffected != 1)
            {
                throw new InvalidOperationException(
                    $"Delete failed, expected to update 1 record but updated {recordsAffected}");
            }
        }
    }
}
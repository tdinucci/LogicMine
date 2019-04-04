using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject
{
    /// <inheritdoc />
    public interface IDataObjectStore<T, TId> : IDataObjectStore<T>
    {
        /// <summary>
        /// Retrieve an object by id
        /// </summary>
        /// <param name="id">The id of the required object</param>
        /// <param name="fields">The fields to select.  If null/empty then all fields are selected</param>
        /// <returns>The T with the given id</returns>
        Task<T> GetByIdAsync(TId id, string[] fields = null);

        /// <summary>
        /// Create a new T
        /// </summary>
        /// <param name="obj">The object to create</param>
        /// <returns>The identity of the new object</returns>
        Task<TId> CreateAsync(T obj);

        /// <summary>
        /// Update an existing object
        /// </summary>
        /// <param name="id">The identity of the object to update</param>
        /// <param name="modifiedProperties">The properties to modify on the object</param>
        /// <returns></returns>
        Task UpdateAsync(TId id, IDictionary<string, object> modifiedProperties);

        /// <summary>
        /// Delete a T
        /// </summary>
        /// <param name="id">The id of the object to delete</param>
        /// <returns></returns>
        Task DeleteAsync(TId id);
    }

    /// <summary>
    /// A store for data objects
    /// </summary>
    /// <typeparam name="T">The data type the store handles</typeparam>
    public interface IDataObjectStore<T>
    {
        /// <summary>
        /// Retrieve a collection of T
        /// </summary>
        /// <param name="max">The maximum number of T's to retrieve</param>
        /// <param name="page">The page of results to retrieve</param>
        /// <param name="fields">The fields to select.  If null/empty then all fields are selected</param>
        /// <returns>A collection of T</returns>
        Task<T[]> GetCollectionAsync(int? max = null, int? page = null, string[] fields = null);

        /// <summary>
        /// Retrieve a collection of T
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        /// <param name="max">The maximum number of T's to retrieve</param>
        /// <param name="page">The page of results to retrieve</param>
        /// <param name="fields">The fields to select.  If null/empty then all fields are selected</param>
        /// <returns>A collection of T</returns>
        Task<T[]> GetCollectionAsync(IFilter<T> filter, int? max = null, int? page = null, string[] fields = null);

        /// <summary>
        /// Create a new collection of T
        /// </summary>
        /// <param name="objs">The objects to create</param>
        /// <returns></returns>
        Task CreateCollectionAsync(IEnumerable<T> objs);

        /// <summary>
        /// Delete a collection of T
        /// </summary>
        /// <param name="filter">The filter to apply to the set of T</param>
        Task DeleteCollectionAsync(IFilter<T> filter);
    }
}
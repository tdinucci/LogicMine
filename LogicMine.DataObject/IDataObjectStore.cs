using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine.DataObject.Filter;

namespace LogicMine.DataObject
{
    public interface IDataObjectStore<T, TId> : IDataObjectStore<T>
    {
        Task<T> GetByIdAsync(TId id);
        Task<TId> CreateAsync(T obj);
        Task UpdateAsync(TId id, IDictionary<string, object> modifiedProperties);
        Task DeleteAsync(TId id);
    }

    public interface IDataObjectStore<T>
    {
        Task<T[]> GetCollectionAsync(int? max = null, int? page = null);
        Task<T[]> GetCollectionAsync(IFilter<T> filter, int? max = null, int? page = null);
    }
}
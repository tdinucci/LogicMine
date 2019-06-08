using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastMember;
using LogicMine.DataObject.Filter;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LogicMine.DataObject.MongoDb
{
    /// <summary>
    /// A MongoDb store for data objects
    /// </summary>
    /// <typeparam name="T">The data type the store handles</typeparam>
    public class MongoDbObjectStore<T> : IDataObjectStore<T, Guid>
        where T : new()
    {
        private readonly TypeAccessor _typeAccessor;
        private readonly MemberSet _typeMembers;

        protected IMongoDatabase MongoDb { get; }
        protected MongoDbObjectDescriptor<T> Descriptor { get; }
        protected ITransientErrorAwareExecutor TransientErrorAwareExecutor { get; }

        public MongoDbObjectStore(IMongoDatabase mongoDb, MongoDbObjectDescriptor<T> descriptor,
            ITransientErrorAwareExecutor transientErrorAwareExecutor = null)
        {
            MongoDb = mongoDb ?? throw new ArgumentNullException(nameof(mongoDb));
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            TransientErrorAwareExecutor = transientErrorAwareExecutor;

            _typeAccessor = TypeAccessor.Create(typeof(T));
            _typeMembers = _typeAccessor.GetMembers();
        }

        /// <inheritdoc />
        public async Task<Guid> CreateAsync(T obj)
        {
            var create = new Func<Task>(() => MongoDb.GetCollection<T>(Descriptor.CollectionName).InsertOneAsync(obj));

            if (TransientErrorAwareExecutor == null)
                await create().ConfigureAwait(false);
            else
                await TransientErrorAwareExecutor.ExecuteAsync(create).ConfigureAwait(false);

            return Descriptor.GetId(obj);
        }

        /// <inheritdoc />
        public Task CreateCollectionAsync(IEnumerable<T> objs)
        {
            var create = new Func<Task>(
                () => MongoDb.GetCollection<T>(Descriptor.CollectionName).InsertManyAsync(objs));

            return TransientErrorAwareExecutor == null
                ? create()
                : TransientErrorAwareExecutor.ExecuteAsync(create);
        }

        /// <inheritdoc />
        public async Task<T> GetByIdAsync(Guid id, string[] fields = null)
        {
            var filter = Builders<T>.Filter.Eq(Descriptor.GetIdPropertyName(), id);

            var get = new Func<Task<T>>(async () =>
            {
                var docs = await MongoDb.GetCollection<T>(Descriptor.CollectionName).FindAsync(filter);
                return docs.FirstOrDefault();
            });

            var obj = TransientErrorAwareExecutor == null
                ? await get().ConfigureAwait(false)
                : await TransientErrorAwareExecutor.ExecuteAsync(get).ConfigureAwait(false);

            if (obj == null)
                throw new InvalidOperationException($"No '{typeof(T).Name}' found with id '{id}'");

            return ClearUnreadableProperties(obj, fields);
        }

        /// <inheritdoc />
        public Task<T[]> GetCollectionAsync(int? max = null, int? page = null, string[] fields = null)
        {
            return GetCollectionAsync(null, max, page, fields);
        }

        /// <inheritdoc />
        public async Task<T[]> GetCollectionAsync(IFilter<T> filter, int? max = null, int? page = null,
            string[] fields = null)
        {
            var dbFilter = filter == null
                ? new BsonDocument()
                : new MongoDbFilterGenerator<T>(filter, Descriptor.GetMappedColumnName).Generate();

            var finder = MongoDb.GetCollection<T>(Descriptor.CollectionName).Find(dbFilter);
            if (max.GetValueOrDefault(0) > 0)
            {
                finder.Limit(max);
                if (page.GetValueOrDefault(0) > 0)
                    finder.Skip(page * max);
            }

            var get = new Func<Task<T[]>>(async () =>
            {
                using (var cursor = await finder.ToCursorAsync().ConfigureAwait(false))
                {
                    return cursor.ToEnumerable().Select(o => ClearUnreadableProperties(o, fields)).ToArray();
                }
            });

            return TransientErrorAwareExecutor == null
                ? await get().ConfigureAwait(false)
                : await TransientErrorAwareExecutor.ExecuteAsync(get).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Guid id, IDictionary<string, object> modifiedProperties)
        {
            var filter = Builders<T>.Filter.Eq(Descriptor.GetIdPropertyName(), id);
            var updateBuilder = Builders<T>.Update;
            var updates = new List<UpdateDefinition<T>>();

            modifiedProperties = modifiedProperties.ToDictionary(p => p.Key.ToLower(), p => p.Value);
            foreach (var member in _typeMembers.Where(m => modifiedProperties.ContainsKey(m.Name.ToLower())))
            {
                if (!Descriptor.CanWrite(member.Name))
                    throw new InvalidOperationException($"Cannot write to '{member.Name}'");

                var fieldName = Descriptor.GetMappedColumnName(member.Name);
                if (!string.IsNullOrWhiteSpace(fieldName))
                {
                    var value = modifiedProperties[member.Name.ToLower()];
                    updates.Add(updateBuilder.Set(fieldName, value));
                }
            }

            var update = new Func<Task<UpdateResult>>(
                () => MongoDb.GetCollection<T>(Descriptor.CollectionName)
                    .UpdateOneAsync(filter, updateBuilder.Combine(updates)));

            var result = TransientErrorAwareExecutor == null
                ? await update().ConfigureAwait(false)
                : await TransientErrorAwareExecutor.ExecuteAsync(update);

            if (result.ModifiedCount != 1)
                throw new InvalidOperationException($"Failed to update '{typeof(T).Name}' with id '{id}'");
        }

        /// <inheritdoc />
        public async Task DeleteCollectionAsync(IFilter<T> filter)
        {
            var dbFilter = new MongoDbFilterGenerator<T>(filter, Descriptor.GetMappedColumnName).Generate();

            var delete = new Func<Task<DeleteResult>>(() =>
                MongoDb.GetCollection<T>(Descriptor.CollectionName).DeleteManyAsync(dbFilter));

            var result = TransientErrorAwareExecutor == null
                ? await delete().ConfigureAwait(false)
                : await TransientErrorAwareExecutor.ExecuteAsync(delete).ConfigureAwait(false);

            if (!result.IsAcknowledged)
                throw new InvalidOperationException($"Failed to delete collection of '{typeof(T).Name}'");
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq(Descriptor.GetIdPropertyName(), id);

            var delete = new Func<Task<DeleteResult>>(
                () => MongoDb.GetCollection<T>(Descriptor.CollectionName).DeleteOneAsync(filter));

            var result = TransientErrorAwareExecutor == null
                ? await delete().ConfigureAwait(false)
                : await TransientErrorAwareExecutor.ExecuteAsync(delete).ConfigureAwait(false);

            if (result.DeletedCount != 1)
                throw new InvalidOperationException($"Failed to delete '{typeof(T).Name}' with id '{id}'");
        }

        private T ClearUnreadableProperties(T obj, string[] desiredFields)
        {
            foreach (var member in _typeMembers)
            {
                if (!Descriptor.CanRead(member.Name) ||
                    (desiredFields?.Length > 0 &&
                     !desiredFields.Contains(member.Name, StringComparer.OrdinalIgnoreCase)))
                {
                    if (member.Type.IsValueType)
                        _typeAccessor[obj, member.Name] = Activator.CreateInstance(member.Type);
                    else
                        _typeAccessor[obj, member.Name] = null;
                }
            }

            return obj;
        }
    }
}
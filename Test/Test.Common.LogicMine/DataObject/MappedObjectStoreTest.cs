using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicMine.DataObject;
using LogicMine.DataObject.Filter;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.Common.LogicMine.DataObject
{
    public abstract class MappedObjectStoreTest<TFrog, TId> where TFrog : Frog<TId>
    {
        protected abstract IDataObjectStore<TFrog, TId> GetStore();

        protected abstract TFrog CreateFrog(int index, string name, DateTime dateOfBirth);
        protected abstract void DeleteAll();

        private void InsertFrogs(IDataObjectStore<TFrog, TId> store, int count)
        {
            DeleteAll();

            var tasks = new Task[count];
            for (var i = 1; i <= count; i++)
            {
                var frog = CreateFrog(i, $"Frank{i}", DateTime.Today.AddDays(-i));
                tasks[i - 1] = store.CreateAsync(frog);
            }

            Task.WaitAll(tasks);
        }

        [Fact]
        public void Get()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 10);

                var collection = store.GetCollectionAsync(5, 0).GetAwaiter().GetResult();
                Assert.True(collection.Length == 5);

                var id = collection.Last().Id;
                var frog = store.GetByIdAsync(id).GetAwaiter().GetResult();

                Assert.Equal(id, frog.Id);
                Assert.StartsWith("Frank", frog.Name);

                var index = int.Parse(frog.Name.Replace("Frank", ""));
                Assert.Equal(DateTime.Today.AddDays(-index), frog.DateOfBirth);
            }
        }

        [Fact]
        public void GetAll()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 100);

                var frogs = store.GetCollectionAsync().GetAwaiter().GetResult();
                Assert.True(frogs.Length == 100);
            }
        }

        [Fact]
        public void GetFiltered()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 100);

                var filter = new Filter<TFrog>(new[]
                {
                    new FilterTerm(nameof(Frog<TId>.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))
                });

                var frogs = store.GetCollectionAsync(filter).GetAwaiter().GetResult();
                Assert.True(frogs.Length == 50);
            }
        }

        [Fact]
        public void GetPaged()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 100);

                TFrog[] frogs;
                for (var i = 0; i < 16; i++)
                {
                    frogs = store.GetCollectionAsync(6, i).GetAwaiter().GetResult();
                    Assert.True(frogs.Length == 6);
                }

                frogs = store.GetCollectionAsync(6, 16).GetAwaiter().GetResult();
                Assert.True(frogs.Length == 4);
            }
        }

        [Fact]
        public void GetFilteredPaged()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 100);

                var filter = new Filter<TFrog>(new[]
                {
                    new FilterTerm(nameof(Frog<TId>.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))
                });

                TFrog[] frogs;
                for (var i = 0; i < 8; i++)
                {
                    frogs = store.GetCollectionAsync(filter, 6, i).GetAwaiter().GetResult();
                    Assert.True(frogs.Length == 6);
                }

                frogs = store.GetCollectionAsync(filter, 6, 8).GetAwaiter().GetResult();
                Assert.True(frogs.Length == 2);
            }
        }

        [Fact]
        public void Update()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 10);

                var collection = store.GetCollectionAsync(7).GetAwaiter().GetResult();
                Assert.True(collection.Length == 7);

                var id = collection.Last().Id;
                store.UpdateAsync(id, new Dictionary<string, object> {{nameof(Frog<TId>.Name), "Patched"}})
                    .GetAwaiter().GetResult();

                var frogs = store.GetCollectionAsync().GetAwaiter().GetResult();

                var seenPatched = false;
                Assert.True(frogs.Length == 10);
                foreach (var frog in frogs)
                {
                    if (frog.Id.Equals(id))
                    {
                        Assert.Equal("Patched", frog.Name);
                        seenPatched = true;
                    }
                    else
                        Assert.NotEqual("Patched", frog.Name);
                }

                Assert.True(seenPatched);
            }
        }

        [Fact]
        public void Delete()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 10);

                var collection = store.GetCollectionAsync(4).GetAwaiter().GetResult();
                Assert.True(collection.Length == 4);

                var id = collection.Last().Id;
                store.DeleteAsync(id).GetAwaiter().GetResult();
                var frogs = store.GetCollectionAsync().GetAwaiter().GetResult();

                Assert.True(frogs.Length == 9);
                foreach (var frog in frogs)
                    Assert.NotEqual(id, frog.Id);
            }
        }

        [Fact]
        public void Create()
        {
            lock (GlobalLocker.Lock)
            {
                var store = GetStore();
                InsertFrogs(store, 0);

                var name = Guid.NewGuid().ToString();
                var frog = CreateFrog(1, name, DateTime.Today.AddDays(-150));
                
                frog.Id = store.CreateAsync(frog).GetAwaiter().GetResult();
                
                var readFrog = store.GetByIdAsync(frog.Id).GetAwaiter().GetResult();

                Assert.Equal(frog, readFrog);
            }
        }
    }
}
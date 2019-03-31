using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.UpdateObject;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.LogicMine.DataObject
{
    public class DataObjectShaftRegistrarTest
    {
        [Fact]
        public async Task RegisterShafts()
        {
            var mine = new Mine();
            var registrar = new MyDataObjectShaftRegistrar();
            registrar.RegisterShafts(mine);

            var response = await mine.SendAsync(new GetObjectRequest<AltFrog, int>(1)).ConfigureAwait(false);
            Assert.False(string.IsNullOrEmpty(response.RequestId.ToString()));
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal("Get", response.Error);

            response = await mine.SendAsync(new GetCollectionRequest<AltFrog>()).ConfigureAwait(false);
            Assert.False(string.IsNullOrEmpty(response.RequestId.ToString()));
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal("GetCollection", response.Error);

            response = await mine.SendAsync(new CreateObjectRequest<AltFrog>(new AltFrog())).ConfigureAwait(false);
            Assert.False(string.IsNullOrEmpty(response.RequestId.ToString()));
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal("Create", response.Error);

            response = await mine.SendAsync(new UpdateObjectRequest<AltFrog, int>(5, new Dictionary<string, object>()
                {
                    {nameof(AltFrog.FrogName), "Kermit"}
                }))
                .ConfigureAwait(false);
            Assert.False(string.IsNullOrEmpty(response.RequestId.ToString()));
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal("Update", response.Error);

            response = await mine.SendAsync(new DeleteObjectRequest<AltFrog, int>(5)).ConfigureAwait(false);
            Assert.False(string.IsNullOrEmpty(response.RequestId.ToString()));
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.Equal("Delete", response.Error);
        }

        private class MyDataObjectShaftRegistrar : DataObjectShaftRegistrar<AltFrog, int>
        {
            protected override IShaft<TRequest, TResponse> GetBasicShaft<TRequest, TResponse>(
                ITerminal<TRequest, TResponse> terminal)
            {
                return new Shaft<TRequest, TResponse>(terminal, new MyDefaultStation());
            }

            protected override IDataObjectStore<AltFrog, int> GetDataObjectStore()
            {
                return new MyDataStore();
            }
        }
        
        private class MyDefaultStation : Station<IRequest, IResponse>
        {
            public override Task DescendToAsync(IBasket<IRequest, IResponse> basket)
            {
                return Task.CompletedTask;
            }

            public override Task AscendFromAsync(IBasket<IRequest, IResponse> basket)
            {
                return Task.CompletedTask;
            }
        }
        
        private class MyDataStore : IDataObjectStore<AltFrog, int>
        {
            public Task<AltFrog[]> GetCollectionAsync(int? max = null, int? page = null)
            {
                throw new TestCanceledException("GetCollection");
            }

            public Task<AltFrog[]> GetCollectionAsync(IFilter<AltFrog> filter, int? max = null, int? page = null)
            {
                throw new TestCanceledException("GetCollection");
            }

            public Task CreateCollectionAsync(IEnumerable<AltFrog> objs)
            {
                throw new TestCanceledException("CreateCollection");
            }

            public Task<AltFrog> GetByIdAsync(int id)
            {
                throw new TestCanceledException("Get");
            }

            public Task<int> CreateAsync(AltFrog obj)
            {
                throw new TestCanceledException("Create");
            }

            public Task UpdateAsync(int id, IDictionary<string, object> modifiedProperties)
            {
                throw new TestCanceledException("Update");
            }

            public Task DeleteAsync(int id)
            {
                throw new TestCanceledException("Delete");
            }
        }
    }
}
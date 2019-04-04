using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateCollection;
using LogicMine.DataObject.CreateObject;
using LogicMine.DataObject.DeleteCollection;
using LogicMine.DataObject.DeleteObject;
using LogicMine.DataObject.Filter;
using LogicMine.DataObject.GetCollection;
using LogicMine.DataObject.GetObject;
using LogicMine.DataObject.UpdateObject;
using Test.Common.LogicMine.DataType;
using Xunit;

namespace Test.Common.LogicMine.Mine
{
    public abstract class MineTest<TFrog, TId> where TFrog : Frog<TId>
    {
        protected abstract IDataObjectDescriptor GetDescriptor();
        protected abstract IDataObjectStore<TFrog, TId> GetObjectStore();

        protected abstract TFrog CreateFrog(int index, string name, DateTime dateOfBirth);
        protected abstract void DeleteAll();

        protected virtual bool PerformTransactionTests { get; }
        
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

        private IMine CreateMine(ITraceExporter traceExporter, int frogCount, bool forTransactionTest = false,
            bool failTransaction = false)
        {
            var objectStore = GetObjectStore();
            InsertFrogs(objectStore, frogCount);

            var createShaft = new Shaft<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(traceExporter,
                new CreateObjectTerminal<TFrog, TId>(objectStore),
                new SecurityStation(),
                new CreateFailTransactionStation<TFrog, TId>(failTransaction));

            createShaft.ExecuteWithinTransaction = forTransactionTest;

            return new global::LogicMine.Mine()
                .AddShaft(createShaft)

                .AddShaft(new Shaft<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(traceExporter,
                    new GetObjectTerminal<TFrog, TId>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(traceExporter,
                    new GetCollectionTerminal<TFrog>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<UpdateObjectRequest<TFrog, TId>, UpdateObjectResponse>(traceExporter,
                    new UpdateObjectTerminal<TFrog, TId>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<DeleteObjectRequest<TFrog, TId>, DeleteObjectResponse>(traceExporter,
                    new DeleteObjectTerminal<TFrog, TId>(objectStore),
                    new SecurityStation()))
                
                .AddShaft(new Shaft<DeleteCollectionRequest<TFrog>, DeleteCollectionResponse>(traceExporter,
                    new DeleteCollectionTerminal<TFrog>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<CreateCollectionRequest<TFrog>, CreateCollectionResponse>(traceExporter,
                    new CreateCollectionTerminal<TFrog>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<ReverseStringRequest, RevereStringResponse>(traceExporter,
                    new ReverseStringTerminal(),
                    new SecurityStation()));
        }

        private async Task<TId> GetRecordIdAsync(IMine mine, int considerCount)
        {
            var request = new GetCollectionRequest<TFrog>(considerCount, 0);
            request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

            var collection = await mine.SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(
                request).ConfigureAwait(false);
            Assert.True(collection.Objects.Length == considerCount);

            return collection.Objects.Last().Id;
        }

        [Fact]
        public void Get()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 10);

                var id = GetRecordIdAsync(mine, 5).GetAwaiter().GetResult();

                var getRequest = new GetObjectRequest<TFrog, TId>(id);
                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                Assert.NotNull(response.Object);
                Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
                Assert.Null(response.Error);
                Assert.Equal(id, response.Object.Id);
                Assert.StartsWith("Frank", response.Object.Name);
            }
        }

        [Fact]
        public void GetWithSelectList()
        {
            lock (GlobalLocker.Lock)
            {
                var selectList = new[] {"Name", "DateOfBirth"};
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 10);

                var id = GetRecordIdAsync(mine, 5).GetAwaiter().GetResult();

                var getRequest = new GetObjectRequest<TFrog, TId>(id, selectList);
                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                Assert.NotNull(response.Object);
                Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
                Assert.Null(response.Error);
                Assert.Equal(default(TId), response.Object.Id);
                Assert.StartsWith("Frank", response.Object.Name);
            }
        }

        [Fact]
        public void GetAll()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 100);

                var request = new GetCollectionRequest<TFrog>();
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                    .GetAwaiter().GetResult();

                Assert.Null(response.Error);
                Assert.True(response.Objects.Length == 100);
            }
        }

        [Fact]
        public void GetAllSelectList()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 100);

                var selectList = new[] {"Id", "DateOfBirth"};
                var request = new GetCollectionRequest<TFrog>(null, null, null, selectList);
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                    .GetAwaiter().GetResult();

                Assert.Null(response.Error);
                Assert.True(response.Objects.Length == 100);
                Assert.DoesNotContain(response.Objects, o => o.Name != null);
                Assert.Empty(response.Objects.Where(o => o.Id.Equals(default(TId))).ToArray());
            }
        }

        [Fact]
        public void GetFiltered()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 100);

                var filter = new Filter<TFrog>(new[]
                {
                    new FilterTerm(nameof(Frog<TId>.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))
                });

                var request = new GetCollectionRequest<TFrog>(filter);
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                    .GetAwaiter().GetResult();

                Assert.Null(response.Error);
                Assert.True(response.Objects.Length == 50);
            }
        }

        [Fact]
        public void GetPaged()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 100);

                GetCollectionRequest<TFrog> request;
                GetCollectionResponse<TFrog> response;
                for (var i = 0; i < 16; i++)
                {
                    request = new GetCollectionRequest<TFrog>(6, i);
                    request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                    response = mine
                        .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                        .GetAwaiter().GetResult();

                    Assert.Null(response.Error);
                    Assert.True(response.Objects.Length == 6);
                }

                request = new GetCollectionRequest<TFrog>(6, 16);
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                response = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                    .GetAwaiter().GetResult();

                Assert.Null(response.Error);
                Assert.True(response.Objects.Length == 4);
            }
        }

        [Fact]
        public void GetFilteredPaged()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 100);

                var filter = new Filter<TFrog>(new[]
                {
                    new FilterTerm(nameof(Frog<TId>.DateOfBirth), FilterOperators.LessThan, DateTime.Today.AddDays(-50))
                });

                GetCollectionRequest<TFrog> request;
                GetCollectionResponse<TFrog> response;
                for (var i = 0; i < 8; i++)
                {
                    request = new GetCollectionRequest<TFrog>(filter, 6, i, null);
                    request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                    response = mine
                        .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                        .GetAwaiter().GetResult();

                    Assert.Null(response.Error);
                    Assert.True(response.Objects.Length == 6);
                }

                request = new GetCollectionRequest<TFrog>(filter, 6, 8, null);
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                response = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                    .GetAwaiter().GetResult();

                Assert.Null(response.Error);
                Assert.True(response.Objects.Length == 2);

            }
        }

        [Fact]
        public void Update()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 10);

                var id = GetRecordIdAsync(mine, 7).GetAwaiter().GetResult();

                var request = new UpdateObjectRequest<TFrog, TId>(id,
                    new Dictionary<string, object> {{nameof(Frog<TId>.Name), "Patched"}});
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<UpdateObjectRequest<TFrog, TId>, UpdateObjectResponse>(request)
                    .GetAwaiter().GetResult();

                Assert.True(response.Success);
                Assert.Null(response.Error);
                Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));

                var getRequest = new GetCollectionRequest<TFrog>();
                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getResponse = mine.SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                var seenPatched = false;
                Assert.True(getResponse.Objects.Length == 10);
                foreach (var frog in getResponse.Objects)
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
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 10);

                var id = GetRecordIdAsync(mine, 4).GetAwaiter().GetResult();

                var request = new DeleteObjectRequest<TFrog, TId>(id);
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<DeleteObjectRequest<TFrog, TId>, DeleteObjectResponse>(request)
                    .GetAwaiter().GetResult();

                Assert.True(response.Success);
                Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
                Assert.Null(response.Error);

                var getCollectionRequest = new GetCollectionRequest<TFrog>();
                getCollectionRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getCollectionResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getCollectionRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getCollectionResponse.Error);
                Assert.True(getCollectionResponse.Objects.Length == 9);

                foreach (var frog in getCollectionResponse.Objects)
                    Assert.NotEqual(id, frog.Id);
            }
        }

        [Fact]
        public virtual void DeleteCollection()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 10);

                // have 10, create 10 more that we'll delete with a filter
                for (var i = 0; i < 10; i++)
                {
                    var frog = CreateFrog(i, $"Kermit{i}", DateTime.Today.AddDays(-150));

                    var createRequest = new CreateObjectRequest<TFrog>(frog);
                    createRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);
                    var createResponse =
                        mine.SendAsync<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(createRequest)
                            .GetAwaiter().GetResult();

                    Assert.Null(createResponse.Error);
                    Assert.False(string.IsNullOrWhiteSpace(createResponse.ObjectId.ToString()));
                }

                // ensure we've 20 items
                var getCollectionRequest = new GetCollectionRequest<TFrog>();
                getCollectionRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getCollectionResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getCollectionRequest)
                    .GetAwaiter().GetResult();
                
                Assert.Null(getCollectionResponse.Error);
                Assert.True(getCollectionResponse.Objects.Length == 20);

                // now do the delete
                var request = new DeleteCollectionRequest<TFrog>(new Filter<TFrog>(new[]
                {
                    new FilterTerm("Name", FilterOperators.StartsWith, "Kermit")
                }));
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine
                    .SendAsync<DeleteCollectionRequest<TFrog>, DeleteCollectionResponse>(request)
                    .GetAwaiter().GetResult();

                if (!string.IsNullOrWhiteSpace(response.Error))
                    throw new InvalidOperationException(response.Error);
                
                Assert.True(response.Success);
                Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
                Assert.Null(response.Error);

                // and ensure we've now got 10 items
                getCollectionRequest = new GetCollectionRequest<TFrog>();
                getCollectionRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                getCollectionResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getCollectionRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getCollectionResponse.Error);
                Assert.True(getCollectionResponse.Objects.Length == 10);
            }
        }

        [Fact]
        public void Create()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 0);

                var name = Guid.NewGuid().ToString();
                var frog = CreateFrog(1, name, DateTime.Today.AddDays(-150));
                
                var createRequest = new CreateObjectRequest<TFrog>(frog);
                createRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);
                var createResponse =
                    mine.SendAsync<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(createRequest)
                        .GetAwaiter().GetResult();

                Assert.Null(createResponse.Error);
                Assert.False(string.IsNullOrWhiteSpace(createResponse.ObjectId.ToString()));

                frog.Id = createResponse.ObjectId;
                
                var getRequest = new GetObjectRequest<TFrog, TId>(createResponse.ObjectId);
                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getResponse = mine
                    .SendAsync<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getResponse.Error);
                Assert.Equal(frog, getResponse.Object);
            }
        }

        [Fact]
        public void CreateCollection()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 0);

                var frogs = new[]
                {
                    CreateFrog(1, Guid.NewGuid().ToString(), DateTime.Today.AddDays(-8)),
                    CreateFrog(2, Guid.NewGuid().ToString(), DateTime.Today.AddDays(-7)),
                    CreateFrog(3, Guid.NewGuid().ToString(), DateTime.Today.AddDays(-6))
                };

                var createRequest = new CreateCollectionRequest<TFrog>(frogs.ToArray());
                createRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);
                var createResponse =
                    mine.SendAsync<CreateCollectionRequest<TFrog>, CreateCollectionResponse>(createRequest)
                        .GetAwaiter().GetResult();

                Assert.Null(createResponse.Error);
                Assert.True(createResponse.Success);

                var getRequest = new GetCollectionRequest<TFrog>();
                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getResponse.Error);
                Assert.True(getResponse.Objects.Length == 3);
                Assert.Contains(getResponse.Objects, f => f.DateOfBirth == DateTime.Today.AddDays(-8));
                Assert.Contains(getResponse.Objects, f => f.DateOfBirth == DateTime.Today.AddDays(-7));
                Assert.Contains(getResponse.Objects, f => f.DateOfBirth == DateTime.Today.AddDays(-6));
            }
        }

        [Fact]
        public void CreateInTransaction()
        {
            if (!PerformTransactionTests)
                return;

            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 0, true);

                var name = Guid.NewGuid().ToString();
                var frog = CreateFrog(1, name, DateTime.Today.AddDays(-150));

                var createRequest = new CreateObjectRequest<TFrog>(frog);
                createRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);
                var createResponse =
                    mine.SendAsync<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(createRequest)
                        .GetAwaiter().GetResult();

                Assert.Null(createResponse.Error);
                Assert.False(string.IsNullOrWhiteSpace(createResponse.ObjectId.ToString()));

                frog.Id = createResponse.ObjectId;

                var getRequest = new GetObjectRequest<TFrog, TId>(createResponse.ObjectId);
                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getResponse = mine
                    .SendAsync<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getResponse.Error);
                Assert.Equal(frog, getResponse.Object);
            }
        }

        [Fact]
        public void CreateFailTransaction()
        {
            if (!PerformTransactionTests)
                return;

            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 0, true, true);

                var name = Guid.NewGuid().ToString();
                var frog = CreateFrog(1, name, DateTime.Today.AddDays(-150));

                var createRequest = new CreateObjectRequest<TFrog>(frog);
                createRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);
                var createResponse =
                    mine.SendAsync<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(createRequest)
                        .GetAwaiter().GetResult();

                Assert.Equal("A failure has occurred", createResponse.Error);

                var getRequest = new GetCollectionRequest<TFrog>(new Filter<TFrog>(new[]
                {
                    new FilterTerm("Name", FilterOperators.Equal, name)
                }));

                getRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getResponse.Error);
                Assert.True(getResponse.Objects.Length == 0);
            }
        }

        [Fact]
        public void BadAccessToken()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 10);

                var id = GetRecordIdAsync(mine, 1).GetAwaiter().GetResult();

                var request = new GetObjectRequest<TFrog, TId>(id);
                request.Options.Add(SecurityStation.AccessTokenOption, "xxxx");

                var response = mine
                    .SendAsync<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(request)
                    .GetAwaiter().GetResult();

                Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-5));
                Assert.NotNull(response.Error);
                Assert.Null(response.Object);
                Assert.Equal("Invalid access token", response.Error);
            }
        }

        [Fact]
        public void ReverseString()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 0);

                var forward = "hello there";
                var expectedReversed = "ereht olleh";

                var request = new ReverseStringRequest(forward);
                request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var response = mine.SendAsync<ReverseStringRequest, RevereStringResponse>(request)
                    .GetAwaiter().GetResult();

                Assert.Equal(expectedReversed, response.Reversed);
            }
        }

        private class ReverseStringRequest : Request
        {
            public string ToReverse { get; }

            public ReverseStringRequest(string toReverse)
            {
                ToReverse = toReverse;
            }
        }

        private class RevereStringResponse : Response
        {
            public string Reversed { get; }

            public RevereStringResponse(IRequest request) : base(request)
            {
            }

            public RevereStringResponse(IRequest request, string reversed) : this(request)
            {
                Reversed = reversed;
            }
        }

        private class ReverseStringTerminal : Terminal<ReverseStringRequest, RevereStringResponse>
        {
            public override Task AddResponseAsync(IBasket<ReverseStringRequest, RevereStringResponse> basket)
            {
                var reversed = new string(basket.Request.ToReverse.Reverse().ToArray());
                basket.Response = new RevereStringResponse(basket.Request, reversed);

                return Task.CompletedTask;
            }
        }
    }
}
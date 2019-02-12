using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject;
using LogicMine.DataObject.CreateObject;
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

        private IMine CreateMine(ITraceExporter traceExporter, int frogCount)
        {
            var objectStore = GetObjectStore();
            InsertFrogs(objectStore, frogCount);

            return new global::LogicMine.Mine()
                .AddShaft(new Shaft<GetObjectRequest<TFrog, TId>, GetObjectResponse<TFrog>>(traceExporter,
                    new GetObjectTerminal<TFrog, TId>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(traceExporter,
                    new GetCollectionTerminal<TFrog>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(traceExporter,
                    new CreateObjectTerminal<TFrog, TId>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<UpdateObjectRequest<TFrog, TId>, UpdateObjectResponse>(traceExporter,
                    new UpdateObjectTerminal<TFrog, TId>(objectStore),
                    new SecurityStation()))

                .AddShaft(new Shaft<DeleteObjectRequest<TFrog, TId>, DeleteObjectResponse>(traceExporter,
                    new DeleteObjectTerminal<TFrog, TId>(objectStore),
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
                    request = new GetCollectionRequest<TFrog>(filter, 6, i);
                    request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                    response = mine
                        .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(request)
                        .GetAwaiter().GetResult();

                    Assert.Null(response.Error);
                    Assert.True(response.Objects.Length == 6);
                }

                request = new GetCollectionRequest<TFrog>(filter, 6, 8);
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
        public void Create()
        {
            lock (GlobalLocker.Lock)
            {
                var traceExporter = new TestTraceExporter();
                var mine = CreateMine(traceExporter, 0);

                var getCollectionRequest = new GetCollectionRequest<TFrog>();
                getCollectionRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                var getCollectionResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getCollectionRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getCollectionResponse.Error);
                Assert.True(getCollectionResponse.Objects.Length == 0);

                var frogName = "Frogert";
                var dob = DateTime.Now;
                var createRequest = new CreateObjectRequest<TFrog>(CreateFrog(1, frogName, dob));
                createRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);
                var createResponse =
                    mine.SendAsync<CreateObjectRequest<TFrog>, CreateObjectResponse<TFrog, TId>>(createRequest)
                        .GetAwaiter().GetResult();

                Assert.Null(createResponse.Error);
                Assert.False(string.IsNullOrWhiteSpace(createRequest.Id.ToString()));

                getCollectionRequest = new GetCollectionRequest<TFrog>();
                getCollectionRequest.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

                getCollectionResponse = mine
                    .SendAsync<GetCollectionRequest<TFrog>, GetCollectionResponse<TFrog>>(getCollectionRequest)
                    .GetAwaiter().GetResult();

                Assert.Null(getCollectionResponse.Error);
                Assert.True(getCollectionResponse.Objects.Length == 1);

                Assert.Equal(frogName, getCollectionResponse.Objects[0].Name);
                Assert.Equal(dob.Date, getCollectionResponse.Objects[0].DateOfBirth.Date);
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
        public async Task ReverseString()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter, 0);

            var forward = "hello there";
            var expectedReversed = "ereht olleh";

            var request = new ReverseStringRequest(forward);
            request.Options.Add(SecurityStation.AccessTokenOption, SecurityStation.ValidAccessToken);

            var response = await mine.SendAsync<ReverseStringRequest, RevereStringResponse>(request)
                .ConfigureAwait(false);

            Assert.Equal(expectedReversed, response.Reversed);
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
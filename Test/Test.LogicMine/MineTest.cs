using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.GetObject;
using Test.Common.LogicMine;
using Xunit;

namespace Test.LogicMine
{
    public class MineTest
    {
        private const string AccessTokenKey = "AccessToken";
        private const string ValidAccessToken = "1234567890";

        private IMine CreateMine(ITraceExporter traceExporter)
        {
            return new Mine()
                .AddShaft(new DefaultShaft<MyGetObjectRequest<Frog, int>, MyGetObjectResponse<Frog>>(traceExporter,
                    new GetObjectTerminal<Frog>(),
                    new MakeNameUppercaseStation<Frog>()))

                .AddShaft(new DefaultShaft<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Tadpole>>(traceExporter,
                    new GetObjectTerminal<Tadpole>()))

                .AddShaft(new DefaultShaft<MyGetObjectRequest<FroggyTadpole, int>, MyGetObjectResponse<FroggyTadpole>>(
                    traceExporter,
                    new GetFroggyTadpoleTerminal()))
                
                .AddShaft(new DefaultShaft<MyGetObjectRequest<FroggyTadpole_Error, int>, MyGetObjectResponse<FroggyTadpole_Error>>(
                    traceExporter,
                    new GetFroggyTadpoleTerminal_ChildError()));
        }

        [Fact]
        public void AddShaft()
        {
            var traceExporter = new TestTraceExporter();

            var terminal1 = new GetObjectTerminal<Frog>();
            var station1 = new MakeNameUppercaseStation<Frog>();
            var shaft1 = new DefaultShaft<MyGetObjectRequest<Frog, int>, MyGetObjectResponse<Frog>>(traceExporter,
                terminal1, station1);

            var terminal2 = new GetObjectTerminal<Tadpole>();
            var station2 = new MakeNameUppercaseStation<Tadpole>();
            var shaft2 = new DefaultShaft<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Tadpole>>(traceExporter,
                terminal2, station2);

            var mine = new Mine()
                .AddShaft(shaft1)
                .AddShaft(shaft2);

            Assert.Equal(mine, (IMine)shaft1.Within);
            Assert.Equal(shaft1, station1.Within);
            Assert.Equal(shaft1, terminal1.Within);
            Assert.Equal(mine, (IMine)station1.Within.Within);
            Assert.Equal(mine, (IMine)terminal1.Within.Within);

            Assert.Equal(mine, (IMine)shaft2.Within);
            Assert.Equal(shaft2, station2.Within);
            Assert.Equal(shaft2, terminal2.Within);
            Assert.Equal(mine, (IMine)station2.Within.Within);
            Assert.Equal(mine, (IMine)terminal2.Within.Within);
        }

        [Fact]
        public async Task GetObject1()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<Frog, int>(ValidAccessToken, 3);
            var response = await mine.SendAsync<MyGetObjectRequest<Frog, int>, MyGetObjectResponse<Frog>>(request)
                .ConfigureAwait(false);

            Assert.NotNull(response.Object);
            Assert.Equal(3, response.Object.Id);
            Assert.Equal("KERMIT 3", response.Object.Name);
        }

        [Fact]
        public async Task GetObject2()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<Tadpole, int>(ValidAccessToken, 5);
            var response = await mine.SendAsync<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Tadpole>>(request)
                .ConfigureAwait(false);

            Assert.NotNull(response.Object);
            Assert.Equal(5, response.Object.Id);
            Assert.Equal("Taddy 5", response.Object.Name);
        }

        [Fact]
        public async Task GetObject_Basket()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<Tadpole, int>(ValidAccessToken, 5);
            var basket = new MyGetObjectBasket<Tadpole, int>(request);
            await mine.SendAsync(basket).ConfigureAwait(false);

            Assert.NotNull(basket.Response.Object);
            Assert.Equal(5, basket.Response.Object.Id);
            Assert.Equal("Taddy 5", basket.Response.Object.Name);
        }
        
        [Fact]
        public async Task GetBadAccessToken()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<Tadpole, int>("ABC", 5);
            var response = await mine.SendAsync<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Tadpole>>(request)
                .ConfigureAwait(false);

            Assert.Null(response.Object);
            Assert.Equal("Invalid access token", response.Error);
        }

        [Fact]
        public async Task GetBadRequest()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<Tadpole, int>(ValidAccessToken, 5);
            var response = await mine.SendAsync<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Frog>>(request)
                .ConfigureAwait(false);

            Assert.Equal(
                "Expected response to be a 'Test.LogicMine.MineTest+MyGetObjectResponse`1[Test.LogicMine.MineTest+Frog]' but it was a 'Test.LogicMine.MineTest+MyGetObjectResponse`1[Test.LogicMine.MineTest+Tadpole]'",
                response.Error);
        }

        [Fact]
        public async Task ChildRequest()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<FroggyTadpole, int>(ValidAccessToken, 5);
            var response = await mine
                .SendAsync<MyGetObjectRequest<FroggyTadpole, int>, MyGetObjectResponse<FroggyTadpole>>(request)
                .ConfigureAwait(false);

            Assert.NotNull(response.Object);
            Assert.Equal(5, response.Object.Frog.Id);
            Assert.Equal(5, response.Object.Tadpole.Id);

            var expectedTrace = $@"
BasketId: {response.Object.FrogRequestId}
ParentId: {response.RequestId}
Test.LogicMine.MineTest+SecurityStation Down
Test.LogicMine.MineTest+MakeNameUppercaseStation`1[Test.LogicMine.MineTest+Frog] Down
Test.LogicMine.MineTest+GetObjectTerminal`1[Test.LogicMine.MineTest+Frog] Down
Test.LogicMine.MineTest+MakeNameUppercaseStation`1[Test.LogicMine.MineTest+Frog] Up
Test.LogicMine.MineTest+SecurityStation Up
---
BasketId: {response.Object.TadpoleRequestId}
ParentId: {response.RequestId}
Test.LogicMine.MineTest+SecurityStation Down
Test.LogicMine.MineTest+GetObjectTerminal`1[Test.LogicMine.MineTest+Tadpole] Down
Test.LogicMine.MineTest+SecurityStation Up
---
BasketId: {response.RequestId}
Test.LogicMine.MineTest+SecurityStation Down
Test.LogicMine.MineTest+GetFroggyTadpoleTerminal Down
Test.LogicMine.MineTest+SecurityStation Up
---
".TrimStart();

            Assert.Equal(expectedTrace, traceExporter.Trace);
        }

        [Fact]
        public async Task ChildRequest_NonGen()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<FroggyTadpole, int>(ValidAccessToken, 5);
            var response = await mine.SendAsync(request).ConfigureAwait(false) as MyGetObjectResponse<FroggyTadpole>;

            Assert.NotNull(response.Object);
            Assert.Equal(5, response.Object.Frog.Id);
            Assert.Equal(5, response.Object.Tadpole.Id);

            var expectedTrace = $@"
BasketId: {response.Object.FrogRequestId}
ParentId: {response.RequestId}
Test.LogicMine.MineTest+SecurityStation Down
Test.LogicMine.MineTest+MakeNameUppercaseStation`1[Test.LogicMine.MineTest+Frog] Down
Test.LogicMine.MineTest+GetObjectTerminal`1[Test.LogicMine.MineTest+Frog] Down
Test.LogicMine.MineTest+MakeNameUppercaseStation`1[Test.LogicMine.MineTest+Frog] Up
Test.LogicMine.MineTest+SecurityStation Up
---
BasketId: {response.Object.TadpoleRequestId}
ParentId: {response.RequestId}
Test.LogicMine.MineTest+SecurityStation Down
Test.LogicMine.MineTest+GetObjectTerminal`1[Test.LogicMine.MineTest+Tadpole] Down
Test.LogicMine.MineTest+SecurityStation Up
---
BasketId: {response.RequestId}
Test.LogicMine.MineTest+SecurityStation Down
Test.LogicMine.MineTest+GetFroggyTadpoleTerminal Down
Test.LogicMine.MineTest+SecurityStation Up
---
".TrimStart();

            Assert.Equal(expectedTrace, traceExporter.Trace);
        }
        
        [Fact]
        public async Task ChildRequest_Error()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<FroggyTadpole_Error, int>(ValidAccessToken, 5);
            var response = await mine
                .SendAsync<MyGetObjectRequest<FroggyTadpole_Error, int>, MyGetObjectResponse<FroggyTadpole_Error>>(request)
                .ConfigureAwait(false);

            Assert.Null(response.Object);
            Assert.NotEqual(Guid.Empty, response.RequestId);
            Assert.Equal("Child request failed: Invalid access token", response.Error);
        }

        [Fact]
        public async Task ChildRequest_ErrorNonGen()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new MyGetObjectRequest<FroggyTadpole_Error, int>(ValidAccessToken, 5);
            var response =
                await mine.SendAsync(request).ConfigureAwait(false) as MyGetObjectResponse<FroggyTadpole_Error>;

            Assert.Null(response.Object);
            Assert.NotEqual(Guid.Empty, response.RequestId);
            Assert.Equal("Child request failed: Invalid access token", response.Error);
        }

        private class DefaultShaft<TRequest, TResponse> : Shaft<TRequest, TResponse>
            where TRequest : class, IRequest
            where TResponse : IResponse
        {
            public DefaultShaft(ITraceExporter traceExporter, ITerminal<TRequest, TResponse> terminal,
                params IStation<TRequest, TResponse>[] stations) : base(traceExporter, terminal, stations)
            {
                AddToTop(new SecurityStation());
            }
        }

        private class MyGetObjectRequest<T, TId> : Request
        {
            public TId ObjectId { get; }

            public MyGetObjectRequest(TId objectId)
            {
                ObjectId = objectId;
            }
            
            public MyGetObjectRequest(string accessToken, TId objectId)
            {
                Options.Add(AccessTokenKey, accessToken);
                ObjectId = objectId;
            }
        }

        private class MyGetObjectResponse<T> : Response
        {
            public T Object { get; }

            public MyGetObjectResponse(IRequest request) : base(request)
            {
            }
            
            public MyGetObjectResponse(IRequest request, T obj) :base(request)
            {
                Object = obj;
            }
        }

        private class MyGetObjectBasket<T, TId> : Basket<MyGetObjectRequest<T, TId>, MyGetObjectResponse<T>>
        {
            public MyGetObjectBasket(MyGetObjectRequest<T, TId> request) : base(request)
            {
            }
        }

        private class GetObjectTerminal<T> : Terminal<MyGetObjectRequest<T, int>, MyGetObjectResponse<T>> where T : class
        {
            public override Task AddResponseAsync(IBasket<MyGetObjectRequest<T, int>, MyGetObjectResponse<T>> basket)
            {
                var id = basket.Request.ObjectId;
                var db = new Database();

                T result;
                if (typeof(T) == typeof(Frog))
                    result = db.GetFrog(id) as T;
                else if (typeof(T) == typeof(Tadpole))
                    result = db.GetTadpole(id) as T;
                else 
                    throw new InvalidCastException("Unexpected data type requested");

                basket.Response = new MyGetObjectResponse<T>(basket.Request, result);

                return Task.CompletedTask;
            }
        }

        private class GetFroggyTadpoleTerminal : 
            Terminal<MyGetObjectRequest<FroggyTadpole, int>, MyGetObjectResponse<FroggyTadpole>>
        {
            public override async Task AddResponseAsync(
                IBasket<MyGetObjectRequest<FroggyTadpole, int>, MyGetObjectResponse<FroggyTadpole>> basket)
            {
                var id = basket.Request.ObjectId;

                var frogRequest = new MyGetObjectRequest<Frog, int>(id);
                var frogResponse = await Within.Within
                    .SendAsync<MyGetObjectRequest<Frog, int>, MyGetObjectResponse<Frog>>(basket, frogRequest)
                    .ConfigureAwait(false);

                var tadpoleRequest = new MyGetObjectRequest<Tadpole, int>(id);
                var tadpoleResponse = await Within.Within
                    .SendAsync<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Tadpole>>(basket, tadpoleRequest)
                    .ConfigureAwait(false);

                var froggyTadpole = new FroggyTadpole
                {
                    Frog = frogResponse.Object, 
                    Tadpole = tadpoleResponse.Object,
                    
                    FrogRequestId = frogRequest.Id,
                    TadpoleRequestId = tadpoleRequest.Id
                };
                basket.Response = new MyGetObjectResponse<FroggyTadpole>(basket.Request, froggyTadpole);
            }
        }

        private class GetFroggyTadpoleTerminal_ChildError : 
            Terminal<MyGetObjectRequest<FroggyTadpole_Error, int>, MyGetObjectResponse<FroggyTadpole_Error>>
        {
            public override async Task AddResponseAsync(
                IBasket<MyGetObjectRequest<FroggyTadpole_Error, int>, MyGetObjectResponse<FroggyTadpole_Error>> basket)
            {
                var id = basket.Request.ObjectId;

                var frogRequest = new MyGetObjectRequest<Frog, int>(id);
                
                // by passing false for inheritParentOptions we're preventing security info from passing to the child 
                // which should cause an error
                var frogResponse = await Within.Within
                    .SendAsync<MyGetObjectRequest<Frog, int>, MyGetObjectResponse<Frog>>(basket, frogRequest, false)
                    .ConfigureAwait(false);

                var tadpoleRequest = new MyGetObjectRequest<Tadpole, int>(id);
                var tadpoleResponse = await Within.Within
                    .SendAsync<MyGetObjectRequest<Tadpole, int>, MyGetObjectResponse<Tadpole>>(basket, tadpoleRequest, false)
                    .ConfigureAwait(false);

                var froggyTadpole = new FroggyTadpole_Error
                {
                    Frog = frogResponse.Object, 
                    Tadpole = tadpoleResponse.Object,
                    
                    FrogRequestId = frogRequest.Id,
                    TadpoleRequestId = tadpoleRequest.Id
                };
                basket.Response = new MyGetObjectResponse<FroggyTadpole_Error>(basket.Request, froggyTadpole);
            }
        }
        
        private class MakeNameUppercaseStation<T> : Station<MyGetObjectRequest<T, int>, MyGetObjectResponse<T>>
            where T : class, INamed
        {
            public override Task DescendToAsync(IBasket<MyGetObjectRequest<T, int>, MyGetObjectResponse<T>> basket)
            {
                return Task.CompletedTask;
            }

            public override Task AscendFromAsync(IBasket<MyGetObjectRequest<T, int>, MyGetObjectResponse<T>> basket)
            {
                basket.Response.Object.Name = basket.Response.Object.Name.ToUpper();

                return Task.CompletedTask;
            }
        }

        private class SecurityStation : Station<IRequest, IResponse>
        {
            public override Task DescendToAsync(IBasket<IRequest, IResponse> basket)
            {
                if (basket.Request.Options.TryGetValue(AccessTokenKey, out var accessToken))
                {
                    if ((string) accessToken == ValidAccessToken)
                        return Task.CompletedTask;
                }

                throw new InvalidOperationException("Invalid access token");
            }

            public override Task AscendFromAsync(IBasket<IRequest, IResponse> basket)
            {
                return Task.CompletedTask;
            }
        }

        private class Database
        {
            private readonly Frog[] _frogs =
            {
                new Frog(1, "Kermit 1"),
                new Frog(2, "Kermit 2"),
                new Frog(3, "Kermit 3"),
                new Frog(4, "Kermit 4"),
                new Frog(5, "Kermit 5"),
            };

            private readonly Tadpole[] _tadpoles =
            {
                new Tadpole(1, "Taddy 1"),
                new Tadpole(2, "Taddy 2"),
                new Tadpole(3, "Taddy 3"),
                new Tadpole(4, "Taddy 4"),
                new Tadpole(5, "Taddy 5"),
            };

            public Frog GetFrog(int id)
            {
                return _frogs.First(f => f.Id == id);
            }

            public Tadpole GetTadpole(int id)
            {
                return _tadpoles.First(t => t.Id == id);
            }
        }

        private class Frog : INamed
        {
            public int Id { get; }
            public string Name { get; set; }

            public Frog(int id, string name)
            {
                if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

                Id = id;
                Name = name;
            }
        }

        private class Tadpole : INamed
        {
            public int Id { get; }
            public string Name { get; set; }

            public Tadpole(int id, string name)
            {
                if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

                Id = id;
                Name = name;
            }
        }

        private class FroggyTadpole
        {
            public Frog Frog { get; set; }
            public Tadpole Tadpole { get; set; }
            
            public Guid FrogRequestId { get; set; }
            public Guid TadpoleRequestId { get; set; }
        }
        
        private class FroggyTadpole_Error : FroggyTadpole
        {
        }
        
        private interface INamed
        {
            string Name { get; set; }
        }
    }
}
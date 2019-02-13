using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
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
                .AddShaft(new DefaultShaft<GetObjectRequest<Frog, int>, GetObjectResponse<Frog>>(traceExporter,
                    new GetObjectTerminal<Frog>(),
                    new MakeNameUppercaseStation<Frog>()))
                .AddShaft(new DefaultShaft<GetObjectRequest<Tadpole, int>, GetObjectResponse<Tadpole>>(traceExporter,
                    new GetObjectTerminal<Tadpole>()));
        }

        [Fact]
        public void AddShaft()
        {
            var traceExporter = new TestTraceExporter();

            var terminal1 = new GetObjectTerminal<Frog>();
            var station1 = new MakeNameUppercaseStation<Frog>();
            var shaft1 = new DefaultShaft<GetObjectRequest<Frog, int>, GetObjectResponse<Frog>>(traceExporter,
                terminal1, station1);

            var terminal2 = new GetObjectTerminal<Tadpole>();
            var station2 = new MakeNameUppercaseStation<Tadpole>();
            var shaft2 = new DefaultShaft<GetObjectRequest<Tadpole, int>, GetObjectResponse<Tadpole>>(traceExporter,
                terminal2, station2);

            var mine = new Mine()
                .AddShaft(shaft1)
                .AddShaft(shaft2);

            Assert.Equal(mine, shaft1.Within);
            Assert.Equal(shaft1, station1.Within);
            Assert.Equal(shaft1, terminal1.Within);
            Assert.Equal(mine, station1.Within.Within);
            Assert.Equal(mine, terminal1.Within.Within);

            Assert.Equal(mine, shaft2.Within);
            Assert.Equal(shaft2, station2.Within);
            Assert.Equal(shaft2, terminal2.Within);
            Assert.Equal(mine, station2.Within.Within);
            Assert.Equal(mine, terminal2.Within.Within);
        }

        [Fact]
        public async Task GetObject1()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new GetObjectRequest<Frog, int>(ValidAccessToken, 3);
            var response = await mine.SendAsync<GetObjectRequest<Frog, int>, GetObjectResponse<Frog>>(request)
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

            var request = new GetObjectRequest<Tadpole, int>(ValidAccessToken, 5);
            var response = await mine.SendAsync<GetObjectRequest<Tadpole, int>, GetObjectResponse<Tadpole>>(request)
                .ConfigureAwait(false);

            Assert.NotNull(response.Object);
            Assert.Equal(5, response.Object.Id);
            Assert.Equal("Taddy 5", response.Object.Name);
        }

        [Fact]
        public async Task GetBadAccessToken()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new GetObjectRequest<Tadpole, int>("ABC", 5);
            var response = await mine.SendAsync<GetObjectRequest<Tadpole, int>, GetObjectResponse<Tadpole>>(request)
                .ConfigureAwait(false);

            Assert.Null(response.Object);
            Assert.Equal("Invalid access token", response.Error);
        }

        [Fact]
        public async Task GetBadRequest()
        {
            var traceExporter = new TestTraceExporter();
            var mine = CreateMine(traceExporter);

            var request = new GetObjectRequest<Tadpole, int>(ValidAccessToken, 5);
            var response = await mine.SendAsync<GetObjectRequest<Tadpole, int>, GetObjectResponse<Frog>>(request)
                .ConfigureAwait(false);

            Assert.Equal(
                "Expected response to be a 'Test.LogicMine.MineTest+GetObjectResponse`1[Test.LogicMine.MineTest+Frog]' but it was a 'Test.LogicMine.MineTest+GetObjectResponse`1[Test.LogicMine.MineTest+Tadpole]'",
                response.Error);
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

        private class GetObjectRequest<T, TId> : Request
        {
            public TId ObjectId { get; }

            public GetObjectRequest(string accessToken, TId objectId)
            {
                Options.Add(AccessTokenKey, accessToken);
                ObjectId = objectId;
            }
        }

        private class GetObjectResponse<T> : Response
        {
            public T Object { get; }

            public GetObjectResponse(IRequest request) : base(request)
            {
            }
            
            public GetObjectResponse(IRequest request, T obj) :base(request)
            {
                Object = obj;
            }
        }

        private class GetObjectTerminal<T> : Terminal<GetObjectRequest<T, int>, GetObjectResponse<T>> where T : class
        {
            public override Task AddResponseAsync(IBasket<GetObjectRequest<T, int>, GetObjectResponse<T>> basket)
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

                basket.Response = new GetObjectResponse<T>(basket.Request, result);

                return Task.CompletedTask;
            }
        }

        private class MakeNameUppercaseStation<T> : Station<GetObjectRequest<T, int>, GetObjectResponse<T>>
            where T : class, INamed
        {
            public override Task DescendToAsync(IBasket<GetObjectRequest<T, int>, GetObjectResponse<T>> basket)
            {
                return Task.CompletedTask;
            }

            public override Task AscendFromAsync(IBasket<GetObjectRequest<T, int>, GetObjectResponse<T>> basket)
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

        private interface INamed
        {
            string Name { get; set; }
        }
    }
}
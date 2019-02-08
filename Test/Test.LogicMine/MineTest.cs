using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicMine;
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
            where TResponse : IResponse, new()
        {
            public DefaultShaft(ITraceExporter traceExporter, ITerminal<TRequest, TResponse> terminal,
                params IStation<TRequest, TResponse>[] stations) : base(traceExporter, terminal, stations)
            {
                AddToTop(new SecurityStation());
            }
        }

        private class GetObjectRequest<T, TId> : Request
        {
            public TId Id { get; }

            public GetObjectRequest(string accessToken, TId id)
            {
                Options.Add(AccessTokenKey, accessToken);
                Id = id;
            }
        }

        private class GetObjectResponse<T> : Response
        {
            public T Object { get; }

            public GetObjectResponse()
            {
            }

            public GetObjectResponse(T obj)
            {
                Object = obj;
            }
        }

        private class GetObjectTerminal<T> : Terminal<GetObjectRequest<T, int>, GetObjectResponse<T>> where T : class
        {
            public override Task AddResponseAsync(IBasket<GetObjectRequest<T, int>, GetObjectResponse<T>> basket)
            {
                var id = basket.Payload.Request.Id;
                var db = new Database();

                T result;
                if (typeof(T) == typeof(Frog))
                    result = db.GetFrog(id) as T;
                else if (typeof(T) == typeof(Tadpole))
                    result = db.GetTadpole(id) as T;
                else
                    throw new InvalidCastException("Unexpected data type requested");

                basket.Payload.Response = new GetObjectResponse<T>(result);

                return Task.CompletedTask;
            }
        }

        private class MakeNameUppercaseStation<T> : Station<GetObjectRequest<T, int>, GetObjectResponse<T>>
            where T : class, INamed
        {
            public override Task DescendToAsync(IBasket basket)
            {
                return Task.CompletedTask;
            }

            public override Task AscendFromAsync(IBasket basket)
            {
                var unwrapped = UnwrapBasketPayload(basket);
                unwrapped.Response.Object.Name = unwrapped.Response.Object.Name.ToUpper();

                return Task.CompletedTask;
            }
        }

        private class SecurityStation : Station<IRequest, IResponse>
        {
            public override Task DescendToAsync(IBasket basket)
            {
                if (basket.Payload.Request.Options.TryGetValue(AccessTokenKey, out var accessToken))
                {
                    if ((string) accessToken == ValidAccessToken)
                        return Task.CompletedTask;
                }

                throw new InvalidOperationException("Invalid access token");
            }

            public override Task AscendFromAsync(IBasket basket)
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

        private class TestTraceExporter : ITraceExporter
        {
            public string Trace { get; private set; }
            public string Error { get; private set; }

            public void Export(IBasket basket)
            {
                var sb = new StringBuilder();
                foreach (var visit in basket.Visits)
                    sb.AppendLine($"{visit.Description} {visit.Direction}");

                Trace = sb.ToString();
            }

            public void ExportError(Exception exception)
            {
                Error = exception.Message;
            }

            public void ExportError(string error)
            {
                Error = error;
            }
        }
    }
}
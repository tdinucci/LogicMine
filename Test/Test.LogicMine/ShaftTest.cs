using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using Test.Common.LogicMine;
using Test.Common.LogicMine.Mine;
using Test.Common.LogicMine.Mine.GetDeconstructedDate;
using Test.Common.LogicMine.Mine.GetTime;
using Xunit;

namespace Test.LogicMine
{
    public class ShaftTest
    {
        private const string OptionKey = "Opt";

        [Fact]
        public void Construct()
        {
            var shaft = new Shaft<GetTimeRequest, GetTimeResponse>(new GetTimeTerminal()) as IShaft;
            Assert.Equal(typeof(GetTimeRequest), shaft.RequestType);
            Assert.Equal(typeof(GetTimeResponse), shaft.ResponseType);

            shaft = new Shaft<GetDeconstructedDateRequest, GetDeconstructedDateRespone>(
                new GetDeconstructedDateTerminal());
            Assert.Equal(typeof(GetDeconstructedDateRequest), shaft.RequestType);
            Assert.Equal(typeof(GetDeconstructedDateRespone), shaft.ResponseType);
        }

        [Fact]
        public async Task AddStations()
        {
            var shaft = new Shaft<GetTimeRequest, GetTimeResponse>(new GetTimeTerminal());

            for (var i = 0; i < 5; i++)
            {
                shaft.AddToTop(new TestStation(i));
                shaft.AddToBottom(new OtherTestStation(i));
            }

            var request = new GetTimeRequest();
            request.Options.Add(OptionKey, string.Empty);
            var response = await shaft.SendAsync(request).ConfigureAwait(false);

            Assert.Equal("vTSvTSvTSvTSvTSvOSvOSvOSvOSvOS^OS^OS^OS^OS^OS^TS^TS^TS^TS^TS", request.Options[OptionKey]);
            Assert.True(response.Time < DateTime.Now && response.Time > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public async Task Trace()
        {
            var exporter = new TestTraceExporter();
            var shaft = new Shaft<GetTimeRequest, GetTimeResponse>(exporter,
                new GetTimeTerminal(),
                new TestStation(1),
                new OtherTestStation(2));

            var response = await shaft.SendAsync(new GetTimeRequest()).ConfigureAwait(false);
            Assert.True(response.Time < DateTime.Now && response.Time > DateTime.Now.AddSeconds(-1));

            var expectedLines = @"
Test.LogicMine.ShaftTest+TestStation Down
	[20:32:56.6074861] DescendToAsync 1
Test.LogicMine.ShaftTest+OtherTestStation Down
	[20:32:56.6086293] DescendToAsync 2
Test.Common.LogicMine.Mine.GetTime.GetTimeTerminal Down
Test.LogicMine.ShaftTest+OtherTestStation Up
	[20:32:56.6127470] AscendFromAsync 2
Test.LogicMine.ShaftTest+TestStation Up
	[20:32:56.6127927] AscendFromAsync 1"
                .Trim()
                .Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();

            var traceLines = exporter.Trace
                .Split('\n')
                .Where(l=>!string.IsNullOrWhiteSpace(l))
                .ToArray();

            Assert.Equal(expectedLines.Length, traceLines.Length);
            for (var i = 0; i < expectedLines.Length; i++)
            {
                var expectedLine = expectedLines[i];
                var traceLine = traceLines[i];

                if (expectedLine.IndexOf(']') > 0)
                {
                    expectedLine = expectedLine.Substring(expectedLine.IndexOf(']'));
                    traceLine = traceLine.Substring(traceLine.IndexOf(']'));
                }

                Assert.Equal(expectedLine, traceLine);
            }
        }

        [Fact]
        public async Task SendAsync_TerminalOnly()
        {
            var shaft = new Shaft<GetTimeRequest, GetTimeResponse>(new GetTimeTerminal());
            var response = await shaft.SendAsync(new GetTimeRequest()).ConfigureAwait(false);

            Assert.NotEqual(Guid.Empty, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.True(response.Time < DateTime.Now && response.Time > DateTime.Now.AddSeconds(-1));
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task SendAsync_WithStation()
        {
            var shaft = new Shaft<GetTimeRequest, GetTimeResponse>(new GetTimeTerminal(), new SecurityStation());
            var response = await shaft.SendAsync(new GetTimeRequest()).ConfigureAwait(false);

            Assert.NotEqual(Guid.Empty, response.RequestId);
            Assert.True(response.Date < DateTime.Now && response.Date > DateTime.Now.AddSeconds(-1));
            Assert.False(response.Time.HasValue);
            Assert.Equal("Invalid access token", response.Error);
        }

        private class OtherTestStation : TestStation
        {
            protected override string Option { get; } = "OS";

            public OtherTestStation(int id) : base(id)
            {
            }
        }

        private class TestStation : Station<IRequest, IResponse>
        {
            protected virtual string Option { get; } = "TS";
            
            private readonly int _id;

            public TestStation(int id)
            {
                _id = id;
            }

            public override Task DescendToAsync(IBasket<IRequest, IResponse> basket)
            {
                basket.CurrentVisit.Log($"{nameof(DescendToAsync)} {_id}");
                
                if (basket.Request.Options.ContainsKey(OptionKey))
                    basket.Request.Options[OptionKey] += $"v{Option}";
                
                return Task.CompletedTask;
            }

            public override Task AscendFromAsync(IBasket<IRequest, IResponse> basket)
            {
                basket.CurrentVisit.Log($"{nameof(AscendFromAsync)} {_id}");

                if (basket.Request.Options.ContainsKey(OptionKey))
                    basket.Request.Options[OptionKey] += $"^{Option}";
                
                return Task.CompletedTask;
            }
        }
    }
}
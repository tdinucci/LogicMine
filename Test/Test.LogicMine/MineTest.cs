using System.Collections.Generic;
using System.Reflection;
using LogicMine;
using Test.LogicMine.Common.Layers;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine
{
  public class MineTest
  {
    [Fact]
    public void Construct()
    {
      var mine = new TestMine()
        .Add(new TestStationLayer2())
        .Add(new TestStationLayer3());

      Assert.Equal(typeof(Frog), mine.DataType);

      var terminalLayerProperty = typeof(TestMine).GetProperty("TerminalLayer", BindingFlags.Instance | BindingFlags.NonPublic);
      var terminalLayer = terminalLayerProperty.GetValue(mine);
      Assert.Equal(typeof(SimpleFrogTerminalLayer), terminalLayer.GetType());

      var stationLayersProperty = typeof(TestMine).GetProperty("StationLayers", BindingFlags.Instance | BindingFlags.NonPublic);
      var stationLayers = stationLayersProperty.GetValue(mine) as IReadOnlyCollection<IStationLayer<Frog>>;

      Assert.Equal(3, stationLayers.Count);
      Assert.Contains(stationLayers, l => l.GetType() == typeof(TestStationLayer));
      Assert.Contains(stationLayers, l => l.GetType() == typeof(TestStationLayer2));
      Assert.Contains(stationLayers, l => l.GetType() == typeof(TestStationLayer3));
    }

    private class TestMine : Mine<Frog>
    {
      public TestMine() : base(new SimpleFrogTerminalLayer())
      {
        Add(new TestStationLayer());
      }
    }

    private class TestStationLayer : IStationLayer<Frog>
    {
    }

    private class TestStationLayer2 : IStationLayer<Frog>
    {
    }

    private class TestStationLayer3 : IStationLayer<Frog>
    {
    }
  }
}

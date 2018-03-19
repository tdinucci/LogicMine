//using System.Threading.Tasks;
//using LogicMine;
//using LogicMine.Api;
//using LogicMine.Api.Get;
//using Test.LogicMine.Common.Layers;
//using Test.LogicMine.Common.Types;
//using Xunit;

//namespace Test.LogicMine.Api
//{
//  public class MineRegistryTest
//  {
//    [Fact]
//    public async Task MineAccess()
//    {
//      var registry = new MineRegistry()
//        .Register(new FrogMine())
//        .Register(new AltFrogMine());

//      var frogMine = registry.Access<FrogMine>();
//      var getFrogBasket = new GetBasket<int, Frog>(85);

//      await frogMine.RunAsync(getFrogBasket).ConfigureAwait(false);

//      Assert.Equal(85, getFrogBasket.AscentPayload.Id);
//      Assert.Equal("Kermit", getFrogBasket.AscentPayload.Name);

//      var altFrogMine = registry.Access<AltFrogMine>();
//      var getAltFrogBasket = new GetBasket<int, AltFrog>(999);

//      await altFrogMine.RunAsync(getAltFrogBasket).ConfigureAwait(false);

//      Assert.Equal(999, getAltFrogBasket.AscentPayload.FrogId);
//      Assert.Equal("Kermit", getAltFrogBasket.AscentPayload.FrogName);
//    }

//    [Fact]
//    public async Task Run()
//    {
//      var registry = new MineRegistry()
//        .Register(new FrogMine())
//        .Register(new AltFrogMine());

//      var getFrogBasket = await registry.IssueRequestAsync(new GetBasket<int, Frog>(69));
//      Assert.Equal(69, getFrogBasket.AscentPayload.Id);
//      Assert.Equal("Kermit", getFrogBasket.AscentPayload.Name);

//      var getAltFrogBasket = await registry.IssueRequestAsync(new GetBasket<int, AltFrog>(852));
//      Assert.Equal(852, getAltFrogBasket.AscentPayload.FrogId);
//      Assert.Equal("Kermit", getAltFrogBasket.AscentPayload.FrogName);
//    }

//    private class FrogMine : Mine<Frog>, IGetShaft<int, Frog>
//    {
//      private new SimpleFrogTerminalLayer TerminalLayer => (SimpleFrogTerminalLayer) base.TerminalLayer;

//      public FrogMine() : base(new SimpleFrogTerminalLayer())
//      {
//      }

//      public Task RunAsync(IGetBasket<int, Frog> basket)
//      {
//        return new Shaft<IGetBasket<int, Frog>>(TerminalLayer).RunAsync(basket);
//      }
//    }

//    private class AltFrogMine : Mine<AltFrog>, IGetShaft<int, AltFrog>
//    {
//      private new SimpleAltFrogTerminalLayer TerminalLayer => (SimpleAltFrogTerminalLayer) base.TerminalLayer;

//      public AltFrogMine() : base(new SimpleAltFrogTerminalLayer())
//      {
//      }

//      public Task RunAsync(IGetBasket<int, AltFrog> basket)
//      {
//        return new Shaft<IGetBasket<int, AltFrog>>(TerminalLayer).RunAsync(basket);
//      }
//    }
//  }
//}

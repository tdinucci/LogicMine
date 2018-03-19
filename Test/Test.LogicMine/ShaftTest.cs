using System;
using System.Linq;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Get;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine
{
  public class ShaftTest
  {
    [Fact]
    public async Task Simple()
    {
      var shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal(), null);

      var basket = new GetBasket<int, Frog>(18);
      await shaft.SendAsync(basket).ConfigureAwait(false);

      Assert.Equal(typeof(OkNote), basket.Note.GetType());
      Assert.Equal(18, basket.AscentPayload.Id);
      Assert.Equal("Frank", basket.AscentPayload.Name);
      Assert.Equal(DateTime.Today, basket.AscentPayload.DateOfBirth);

      Assert.Equal(18, basket.DescentPayload);
    }

    [Fact]
    public async Task Pipeline()
    {
      var shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal())
        .Add(new TestGetStation1())
        .Add(new TestGetStation2());

      var basket = new GetBasket<int, Frog>(22);
      await shaft.SendAsync(basket).ConfigureAwait(false);

      Assert.Equal(22, basket.AscentPayload.Id);
      Assert.Equal("Frank21", basket.AscentPayload.Name);
      Assert.Equal(DateTime.Today, basket.AscentPayload.DateOfBirth);
      Assert.Equal(22, basket.DescentPayload);

      shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal())
        .Add(new TestGetStation2())
        .Add(new TestGetStation1());

      basket = new GetBasket<int, Frog>(33);
      await shaft.SendAsync(basket).ConfigureAwait(false);

      Assert.Equal(33, basket.AscentPayload.Id);
      Assert.Equal("Frank12", basket.AscentPayload.Name);
      Assert.Equal(DateTime.Today, basket.AscentPayload.DateOfBirth);
      Assert.Equal(33, basket.DescentPayload);
    }

    [Fact]
    public async Task ReturnEarly()
    {
      var shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal())
        .Add(new TestGetStation1())
        .Add(new TestGetStation2())
        .Add(new TestGetStationReturnEarly());

      var basket = new GetBasket<int, Frog>(22);
      await shaft.SendAsync(basket).ConfigureAwait(false);

      Assert.True(basket.Note is ReturnNote);

      Assert.Equal(22, basket.AscentPayload.Id);
      Assert.Equal("Early Frog", basket.AscentPayload.Name);
      Assert.Equal(DateTime.Today.AddDays(-1), basket.AscentPayload.DateOfBirth);
    }

    [Fact]
    public async Task Trace()
    {
      var shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal())
        .Add(new TestGetStation1())
        .Add(new TestGetStation2());

      var basket = new GetBasket<int, Frog>(22);
      await shaft.SendAsync(basket).ConfigureAwait(false);

      Assert.Equal(5, basket.Visits.Count);

      var visits = basket.Visits.ToArray();

      var visit = visits[0];
      Assert.True(visit.Description == typeof(TestGetStation1).ToString() && visit.Direction == VisitDirections.Down &&
                  visit.Duration > TimeSpan.Zero);

      visit = visits[1];
      Assert.True(visit.Description == typeof(TestGetStation2).ToString() && visit.Direction == VisitDirections.Down &&
                  visit.Duration > TimeSpan.Zero);

      visit = visits[2];
      Assert.True(visit.Description == typeof(TestGetTerminal).ToString() && visit.Direction == VisitDirections.Down &&
                  visit.Duration > TimeSpan.Zero);

      visit = visits[3];
      Assert.True(visit.Description == typeof(TestGetStation2).ToString() && visit.Direction == VisitDirections.Up &&
                  visit.Duration > TimeSpan.Zero);

      visit = visits[4];
      Assert.True(visit.Description == typeof(TestGetStation1).ToString() && visit.Direction == VisitDirections.Up &&
                  visit.Duration > TimeSpan.Zero);

      var waypointTimes = visits.Sum(v => v.Duration?.TotalMilliseconds);
      Assert.True(basket.JourneyDuration.TotalMilliseconds >= waypointTimes);
    }

    [Fact]
    public async Task ExceptionDown()
    {
      var shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal())
        .Add(new TestGetStation1())
        .Add(new TestGetStation2())
        .Add(new TestGetStationExceptionDown());

      var wasException = false;
      try
      {
        var basket = new GetBasket<int, Frog>(22);
        await shaft.SendAsync(basket).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        wasException = true;
        Assert.Equal("Ex on descent", ex.InnerException.Message);
      }

      Assert.True(wasException);
    }

    [Fact]
    public async Task ExceptionUp()
    {
      var shaft = new Shaft<IGetBasket<int, Frog>>(new TestGetTerminal())
        .Add(new TestGetStation1())
        .Add(new TestGetStation2())
        .Add(new TestGetStationExceptionUp());

      var wasException = false;
      try
      {
        var basket = new GetBasket<int, Frog>(22);
        await shaft.SendAsync(basket).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        wasException = true;
        Assert.Equal("Ex on ascent", ex.InnerException.Message);
      }

      Assert.True(wasException);
    }

    private class TestGetTerminal : ITerminal<IBasket<int, Frog>>
    {
      public Task AddResultAsync(IBasket<int, Frog> basket)
      {
        basket.AscentPayload = new Frog {Id = basket.DescentPayload, Name = "Frank", DateOfBirth = DateTime.Today};

        return Task.CompletedTask;
      }
    }

    private class TestGetStation1 : IStation<IBasket<int, Frog>>
    {
      public Task DescendToAsync(IBasket<int, Frog> basket)
      {
        return Task.CompletedTask;
      }

      public Task AscendFromAsync(IBasket<int, Frog> basket)
      {
        basket.AscentPayload.Name += "1";

        return Task.CompletedTask;
      }
    }

    private class TestGetStation2 : IStation<IBasket<int, Frog>>
    {
      public Task DescendToAsync(IBasket<int, Frog> basket)
      {
        return Task.CompletedTask;
      }

      public Task AscendFromAsync(IBasket<int, Frog> basket)
      {
        basket.AscentPayload.Name += "2";

        return Task.CompletedTask;
      }
    }

    private class TestGetStationReturnEarly : IStation<IBasket<int, Frog>>
    {
      public Task DescendToAsync(IBasket<int, Frog> basket)
      {
        basket.AscentPayload = new Frog
        {
          Id = basket.DescentPayload,
          Name = "Early Frog",
          DateOfBirth = DateTime.Today.AddDays(-1)
        };
        basket.ReplaceNote(new ReturnNote());

        return Task.CompletedTask;
      }

      public Task AscendFromAsync(IBasket<int, Frog> basket)
      {
        return Task.CompletedTask;
      }
    }

    private class TestGetStationExceptionDown : IStation<IBasket<int, Frog>>
    {
      public Task DescendToAsync(IBasket<int, Frog> basket)
      {
        throw new InvalidOperationException("Ex on descent");
      }

      public Task AscendFromAsync(IBasket<int, Frog> basket)
      {
        return Task.CompletedTask;
      }
    }

    private class TestGetStationExceptionUp : IStation<IBasket<int, Frog>>
    {
      public Task DescendToAsync(IBasket<int, Frog> basket)
      {
        return Task.CompletedTask;
      }

      public Task AscendFromAsync(IBasket<int, Frog> basket)
      {
        throw new InvalidOperationException("Ex on ascent");
      }
    }
  }
}
using System;
using LogicMine;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine
{
  public class BasketTest
  {
    [Fact]
    public void Construct()
    {
      var frog = new Frog();

      var basket = new TestBasket<Frog>(frog);
      Assert.Equal(frog, basket.DescentPayload);
      Assert.True(basket.Note is OkNote);
      Assert.Equal(0, basket.Visits.Count);
      Assert.Equal(0, basket.Trinkets.Count);
      Assert.Equal(TimeSpan.Zero, basket.JourneyDuration);
    }

    [Fact]
    public void ReplaceNote()
    {
      var note = new ReturnNote();
      var frog = new Frog {Id = 5, Name = "Kermit", DateOfBirth = DateTime.Today};

      var basket = new TestBasket<Frog>(frog);
      Assert.True(basket.Note is OkNote);

      basket.ReplaceNote(note);
      Assert.Same(note, basket.Note);
    }

    [Fact]
    public void AddTrinket()
    {
      var frog = new Frog {Id = 5, Name = "Kermit", DateOfBirth = DateTime.Today};

      var basket = new TestBasket<Frog>(frog);
      Assert.Empty(basket.Trinkets);

      var num = DateTime.Now.Millisecond;
      var date = DateTime.Today;
      var str = Guid.NewGuid().ToString();
      basket.AddTrinket("one", num);
      basket.AddTrinket("two", date);
      basket.AddTrinket("three", str);

      Assert.Equal(3, basket.Trinkets.Count);
      Assert.Equal(num, basket.Trinkets["one"]);
      Assert.Equal(date, basket.Trinkets["two"]);
      Assert.Equal(str, basket.Trinkets["three"]);
    }

    [Fact]
    public void Journey()
    {
      var basket = new TestBasket<Frog>(new Frog());
      basket.AddVisit(new Visit("one", VisitDirections.Down, null, TimeSpan.FromSeconds(1)));
      basket.AddVisit(new Visit("two", VisitDirections.Down, null, TimeSpan.FromSeconds(2)));
      basket.AddVisit(new Visit("three", VisitDirections.Down, null, TimeSpan.FromSeconds(3)));
      basket.AddVisit(new Visit("3", VisitDirections.Up, null, TimeSpan.FromSeconds(3.3)));
      basket.AddVisit(new Visit("2", VisitDirections.Up, null, TimeSpan.FromSeconds(2.2)));
      basket.AddVisit(new Visit("1", VisitDirections.Up, null, TimeSpan.FromSeconds(1.1)));

      basket.JourneyDuration = TimeSpan.FromSeconds(12.6);

      Assert.Equal(12.6, basket.JourneyDuration.TotalSeconds);
      Assert.Equal(6, basket.Visits.Count);

      Assert.Contains(basket.Visits,
        v => v.Description == "one" && v.Direction == VisitDirections.Down && v.Duration == TimeSpan.FromSeconds(1));
      Assert.Contains(basket.Visits,
        v => v.Description == "two" && v.Direction == VisitDirections.Down && v.Duration == TimeSpan.FromSeconds(2));
      Assert.Contains(basket.Visits,
        v => v.Description == "three" && v.Direction == VisitDirections.Down && v.Duration == TimeSpan.FromSeconds(3));

      Assert.Contains(basket.Visits,
        v => v.Description == "3" && v.Direction == VisitDirections.Up && v.Duration == TimeSpan.FromSeconds(3.3));
      Assert.Contains(basket.Visits,
        v => v.Description == "2" && v.Direction == VisitDirections.Up && v.Duration == TimeSpan.FromSeconds(2.2));
      Assert.Contains(basket.Visits,
        v => v.Description == "1" && v.Direction == VisitDirections.Up && v.Duration == TimeSpan.FromSeconds(1.1));
    }

    private class TestBasket<T> : Basket<T, string>
    {
      public override Type DataType => typeof(T);

      public TestBasket(T descentPayload) : base(descentPayload)
      {
      }
    }
  }
}

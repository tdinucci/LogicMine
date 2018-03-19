using System;
using LogicMine.Api.Post;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Post
{
  public class PostBasketTest
  {
    [Fact]
    public void Construct()
    {
      var frog = new Frog { Name = "Fred", DateOfBirth = DateTime.Today };
      var basket = new PostBasket<Frog, int>(frog);
      Assert.Equal(typeof(Frog), basket.DataType);
      Assert.Same(frog, basket.DescentPayload);
    }
  }
}

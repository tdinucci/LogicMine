using System;
using LogicMine.Api.Put;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Put
{
  public class PutRequestTest
  {
    [Fact]
    public void Construct()
    {
      var frog = new Frog {Id = 75, Name = "Fred", DateOfBirth = DateTime.Today};
      var request = new PutRequest<int, Frog>(frog.Id, frog);

      Assert.Equal(75, request.Identity);
      Assert.Same(frog, request.Object);
    }
  }
}

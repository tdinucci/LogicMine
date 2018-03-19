using System;
using System.Collections.Generic;
using LogicMine.Api.Patch;
using Test.LogicMine.Common.Types;
using Xunit;

namespace Test.LogicMine.Api.Patch
{
  public class DeltaTest
  {
    [Fact]
    public void Construct()
    {
      var delta = new Delta<int, Frog>(752, new Dictionary<string, object>
      {
        {nameof(Frog.Name), "abc"},
        {nameof(Frog.DateOfBirth), DateTime.Today}
      });

      Assert.Equal(752, delta.Identity);
      Assert.Equal(2, delta.ModifiedProperties.Count);
      Assert.Contains(delta.ModifiedProperties, m => m.Key == nameof(Frog.Name) && (string) m.Value == "abc");
      Assert.Contains(delta.ModifiedProperties,
        m => m.Key == nameof(Frog.DateOfBirth) && (DateTime) m.Value == DateTime.Today);
    }
  }
}

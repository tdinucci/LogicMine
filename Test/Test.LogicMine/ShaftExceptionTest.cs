using System;
using LogicMine;
using Xunit;

namespace Test.LogicMine
{
  public class ShaftExceptionTest
  {
    [Fact]
    public void ShaftException()
    {
      var message = Guid.NewGuid().ToString();
      var ex = new ShaftException(message);
      Assert.Equal(message, ex.Message);
      Assert.Null(ex.InnerException);

      var message2 = Guid.NewGuid().ToString();
      var ex2 = new ShaftException(message2, ex);
      Assert.Equal(message2, ex2.Message);
      Assert.Equal(message, ex2.InnerException.Message);
    }
  }
}

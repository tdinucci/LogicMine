using LogicMine.Api.Web;
using Xunit;

namespace Test.LogicMine.Api.Web
{
  public class InternalServerErrorObjectResultTest
  {
    [Fact]
    public void Construct()
    {
      var result = new InternalServerErrorObjectResult();
      Assert.Equal(500, result.StatusCode);

      result = new InternalServerErrorObjectResult("some message");
      Assert.Equal(500, result.StatusCode);
      Assert.Equal("some message", result.Value);
    }
  }
}


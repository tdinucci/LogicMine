using System.Threading.Tasks;
using LogicMine;
using LogicMine.Api.Post;

namespace Sample.LogicMine.Common.MatingEvent
{
  /// <summary>
  /// This station is designed to catch baskets that are ascending up a post shaft 
  /// and change the payload within it. 
  /// </summary>
  public class FrenchTranslationStation : IPostStation<IPostBasket<Types.MatingEvent, string>>
  {
    public Task DescendToAsync(IPostBasket<Types.MatingEvent, string> basket, IVisit visit)
    {
      // just let baskets pass through on the way down
      return Task.CompletedTask;
    }

    public Task AscendFromAsync(IPostBasket<Types.MatingEvent, string> basket, IVisit visit)
    {
      // grab baskets on the way up and translate their contents
      var message = basket.AscentPayload;
      if (!string.IsNullOrWhiteSpace(message))
      {
        message = message
          .Replace("and", "et")
          .Replace("had", "ont")
          .Replace("children", "enfants");

        basket.AscentPayload = message;
      }

      return Task.CompletedTask;
    }
  }
}

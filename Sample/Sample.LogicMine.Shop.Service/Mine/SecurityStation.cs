using System;
using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine
{
    /// <summary>
    /// This is a VERY basic station which just ensures that all requests contain a certain access token.
    /// </summary>
    public class SecurityStation : Station<IRequest, IResponse>
    {
        private const string AccessTokenKey = "AccessToken";
        private const string ValidAccessToken = "123";

        public override Task DescendToAsync(IBasket<IRequest, IResponse> basket)
        {
            if (basket.Request.Options.TryGetValue(AccessTokenKey, out var accessToken))
            {
                if ((string) accessToken == ValidAccessToken)
                    return Task.CompletedTask;
            }

            throw new InvalidOperationException("Invalid access token");
        }

        public override Task AscendFromAsync(IBasket<IRequest, IResponse> basket)
        {
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Web.Mine
{
    public class SecurityStation : Station<IRequest, IResponse>
    {
        private const string AccessTokenKey = "AccessToken";
        private const string ValidAccessToken = "123";

        public override Task DescendToAsync(IBasket basket, IBasketPayload<IRequest, IResponse> payload)
        {
            if (payload.Request.Options.TryGetValue(AccessTokenKey, out var accessToken))
            {
                if ((string) accessToken == ValidAccessToken)
                    return Task.CompletedTask;
            }

            throw new InvalidOperationException("Invalid access token");
        }

        public override Task AscendFromAsync(IBasket basket, IBasketPayload<IRequest, IResponse> payload)
        {
            return Task.CompletedTask;
        }
    }
}
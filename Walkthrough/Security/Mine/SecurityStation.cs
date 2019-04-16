using System;
using System.Threading.Tasks;
using LogicMine;

namespace Security.Mine
{
    public class SecurityStation : FlexibleStation<IRequest, IResponse>
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
    }
}
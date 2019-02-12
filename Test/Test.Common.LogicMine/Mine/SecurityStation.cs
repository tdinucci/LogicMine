using System;
using System.Threading.Tasks;
using LogicMine;

namespace Test.Common.LogicMine.Mine
{
    public class SecurityStation : Station<IRequest, IResponse>
    {
        public const string AccessTokenOption = "AccessToken";
        public const string ValidAccessToken = "abc123";

        public override Task DescendToAsync(IBasket<IRequest, IResponse> basket)
        {
            if (basket.Request.Options.TryGetValue(AccessTokenOption, out var accessToken) &&
                accessToken.ToString() == ValidAccessToken)
            {
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
using System;
using System.Threading.Tasks;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.Login
{
    public class LoginTerminal : Terminal<LoginRequest, LoginResponse>
    {
        private const string ExpectedUsername = "eddie";
        private const string ExpectedPassword = "supersecret1";
        private const string AccessToken = "123";

        public override Task AddResponseAsync(IBasket<LoginRequest, LoginResponse> basket)
        {
            if (ExpectedUsername == basket.Request?.Username && ExpectedPassword == basket.Request?.Password)
                basket.Response = new LoginResponse(basket.Request, AccessToken);
            else
                throw new InvalidOperationException("Invalid credentials");

            return Task.CompletedTask;
        }
    }
}
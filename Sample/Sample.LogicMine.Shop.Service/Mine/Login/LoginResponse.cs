using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.Login
{
    // If a login request is successful then a response will be returned which contains an access token
    public class LoginResponse : Response
    {
        public string AccessToken { get; }

        public LoginResponse(IRequest request) : base(request)
        {
        }

        public LoginResponse(IRequest request, string accessToken) : base(request)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));

            AccessToken = accessToken;
        }
    }
}
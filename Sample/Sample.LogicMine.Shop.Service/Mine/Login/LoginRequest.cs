using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.Login
{
    // A request to authenticate with the service
    public class LoginRequest : Request
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
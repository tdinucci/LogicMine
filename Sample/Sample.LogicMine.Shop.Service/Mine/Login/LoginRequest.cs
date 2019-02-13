using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.Login
{
    public class LoginRequest : Request
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
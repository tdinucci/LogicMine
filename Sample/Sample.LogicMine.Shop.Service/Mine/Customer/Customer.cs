using System;

namespace Sample.LogicMine.Shop.Service.Mine.Customer
{
    public class Customer
    {
        public int Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
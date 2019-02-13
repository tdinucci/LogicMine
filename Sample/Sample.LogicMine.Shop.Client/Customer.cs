using System;

namespace Sample.LogicMine.Shop.Client
{
    public class Customer
    {
        public int Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"[{Id}] - {Forename} {Surname} {Email} - Created At {CreatedAt}";
        }
    }
}
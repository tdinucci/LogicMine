using System;

namespace Test.Common.LogicMine.DataType
{
    public class Frog<TId>
    {
        public TId Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Frog<TId> frog &&
                   Id.Equals(frog.Id) &&
                   Name == frog.Name &&
                   DateOfBirth == frog.DateOfBirth;
        }
    }
}
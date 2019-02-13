using System;

namespace Test.Common.LogicMine.DataType
{
#pragma warning disable 659
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
                   DateOfBirth.Date == frog.DateOfBirth.Date;
        }
    }
#pragma warning restore 659
}
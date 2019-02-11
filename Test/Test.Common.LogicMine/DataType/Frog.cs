using System;

namespace Test.Common.LogicMine.DataType
{
  public class Frog
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }

    public override bool Equals(object obj)
    {
      return obj is Frog frog &&
             Id == frog.Id &&
             Name == frog.Name &&
             DateOfBirth == frog.DateOfBirth;
    }
  }
}
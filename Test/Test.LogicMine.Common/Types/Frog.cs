using System;

namespace Test.LogicMine.Common.Types
{
  public class Frog
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }

    public override bool Equals(object obj)
    {
      var frog = obj as Frog;
      return frog != null &&
             Id == frog.Id &&
             Name == frog.Name &&
             DateOfBirth == frog.DateOfBirth;
    }
  }
}
using System;

namespace Sample.LogicMine.Types
{
  /// <summary>
  /// A simple type, where instances are backed up to a database
  /// </summary>
  public class Frog
  {
    public int Id { get; set; }
    public int LivesInPondId { get; set; }
    public int? MotherId { get; set; }
    public int? FatherId { get; set; }
    public bool IsMale { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime DateLastMated { get; set; } = DateTime.MinValue;
  }
}

using System;

namespace Sample.LogicMine.Types
{
  /// <summary>
  /// A simple type, where instances are backed up to a database
  /// </summary>
  public class Tadpole
  {
    public int Id { get; set; }
    public int LivesInPondId { get; set; }
    public int? MotherId { get; set; }
    public int? FatherId { get; set; }
    public bool IsMale { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Call this when you're ready to metamorphasise a tadpole 
    /// </summary>
    /// <returns></returns>
    public Frog ToFrog()
    {
      return new Frog
      {
        LivesInPondId = LivesInPondId,
        MotherId = MotherId,
        FatherId = FatherId,
        DateOfBirth = DateOfBirth,
        IsMale = IsMale,
        Name = Name.Replace("MT", "MF").Replace("FT", "FF")
      };
    }
  }
}

namespace Sample.LogicMine.Types
{
  /// <summary>
  /// This type isn't stored in the database, it's mine will generate instances dynamically 
  /// </summary>
  public class FrogCount
  {
    public int Count { get; set; }

    public FrogCount()
    {
    }

    public FrogCount(int count)
    {
      Count = count;
    }
  }
}

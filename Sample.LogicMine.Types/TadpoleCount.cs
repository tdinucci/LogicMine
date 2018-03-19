namespace Sample.LogicMine.Types
{
  /// <summary>
  /// This type isn't stored in the database, it's mine will generate instances dynamically 
  /// </summary>
  public class TadpoleCount
  {
    public int Count { get; set; }

    public TadpoleCount()
    {
    }

    public TadpoleCount(int count)
    {
      Count = count;
    }
  }
}
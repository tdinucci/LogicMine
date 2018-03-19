using System;

namespace Sample.LogicMine.Types
{
  /// <summary>
  /// Another operational type.  This is used to indicate that we want some process run 
  /// take place which metamorphasises tadpoles
  /// </summary>
  public class MaturationEvent
  {
    public DateTime Date { get; set; }

    public MaturationEvent()
    {
    }

    public MaturationEvent(DateTime date)
    {
      Date = date;
    }
  }
}

using System;

namespace Sample.LogicMine.Types
{
  /// <summary>
  /// This is another type which more for operational purposes than data storage. 
  /// </summary>
  public class MatingEvent
  {
    public Frog Male { get; set; }
    public Frog Female { get; set; }
    public DateTime Date { get; set; }

    public MatingEvent()
    {
    }

    public MatingEvent(Frog male, Frog female, DateTime date)
    {
      Male = male ?? throw new ArgumentNullException(nameof(male));
      Female = female ?? throw new ArgumentNullException(nameof(female));
      Date = date;
    }
  }
}

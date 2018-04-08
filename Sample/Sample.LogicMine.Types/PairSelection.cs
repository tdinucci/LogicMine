using System;

namespace Sample.LogicMine.Types
{
  /// <summary>
  /// Another operational type which indicates that a couple of frogs have hooked up
  /// </summary>
  public class PairSelection
  {
    public Frog Male { get; set; }
    public Frog Female { get; set; }
    public DateTime Date { get; set; }

    public bool IsValid => Male != null && Female != null;

    public PairSelection()
    {
    }

    public PairSelection(Frog male, Frog female, DateTime date)
    {
      Male = male;
      Female = female;
      Date = date;
    }
  }
}

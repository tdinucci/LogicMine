using LogicMine;

namespace Sample.LogicMine.Common
{
  /// <summary>
  /// This is the basis for all mines within this application.  Basically it just makes sure that the top layer 
  /// for every mine is the AppSecurityLayer.
  /// 
  /// This mine does not implement any shafts.  This means if you were to just create an instance of this it wouldn't 
  /// be of any use
  /// </summary>
  public class AppMine<T, TTerminalLayer> : Mine<T> where TTerminalLayer : ITerminalLayer<T>
  {
    protected new TTerminalLayer TerminalLayer => (TTerminalLayer) base.TerminalLayer;

    /// <summary>
    /// Since we're introducing the AppSecurityLayer all mines must capture a "user" so it can be passed to the 
    /// security layer.
    /// </summary>
    public AppMine(string user, TTerminalLayer terminalLayer, ITraceExporter traceExporter) :
      base(terminalLayer, traceExporter)
    {
      // Mines are added from top down, so this will always be the top layer
      Add(new AppSecurityLayer<T>(user));
    }
  }
}

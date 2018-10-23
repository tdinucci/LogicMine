using System;
using LogicMine;
using LogicMine.Api.Security;

namespace Sample.LogicMine.Common
{
  /// <summary>
  /// The base SecurityLayer type will call IsOperationAllowed(...) whenever a shaft hits 
  /// a security station.  In a real application you would reference real user data however here 
  /// we're just checkin that the user is called "kermit".
  /// 
  /// Your own implementation of SecurityLayer may be much more sophisticated than this and do things like;
  /// clear out fields on retrieved objects which a user is not authorised to see, log failed access attempts, etc.
  /// 
  /// N.B. The base SecurityLayer accepts a generic TUser argument.  Here we've set this to string however 
  /// in a real application you may use something else.
  /// </summary>
  public class AppSecurityLayer<T> : SecurityLayer<T, string>
  {
    public AppSecurityLayer(string user) : base(user)
    {
    }

    protected override bool IsOperationAllowed(string user, Operations operation, IBasket basket, IVisit visit)
    {
      return string.Equals(user, "kermit", StringComparison.CurrentCultureIgnoreCase);
    }
  }
}

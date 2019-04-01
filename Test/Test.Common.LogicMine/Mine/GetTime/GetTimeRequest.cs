using System;
using LogicMine;

namespace Test.Common.LogicMine.Mine.GetTime
{
    public class GetTimeRequest : Request
    {
    }

    public class GetDisposableTimeRequest : GetTimeRequest, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
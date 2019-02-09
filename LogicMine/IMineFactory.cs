using System.Collections.Generic;

namespace LogicMine
{
    public interface IMineFactory
    {
        IMine Create(IEnumerable<IShaftRegistrar> shaftRegistrars);
    }
}
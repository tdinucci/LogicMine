using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicMine
{
    public class MineFactory : IMineFactory
    {
        public IMine Create(IEnumerable<IShaftRegistrar> shaftRegistrars)
        {
            if (shaftRegistrars == null || !shaftRegistrars.Any())
                throw new ArgumentException($"There are no entries in '{nameof(shaftRegistrars)}'");

            var mine = new Mine();
            foreach (var shaftRegistrar in shaftRegistrars)
                shaftRegistrar.RegisterShafts(mine);

            return mine;
        }
    }
}
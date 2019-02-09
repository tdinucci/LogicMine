using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicMine
{
    public class MineFactory : IMineFactory
    {
        private readonly IEnumerable<IShaftRegistrar> _shaftRegistrars;

        public MineFactory(IEnumerable<IShaftRegistrar> shaftRegistrars)
        {
            if (shaftRegistrars == null || !shaftRegistrars.Any())
                throw new ArgumentException($"There are no entries in '{nameof(shaftRegistrars)}'");

            _shaftRegistrars = shaftRegistrars;
        }

        public IMine Create()
        {
            var mine = new Mine();
            foreach (var shaftRegistrar in _shaftRegistrars)
                shaftRegistrar.RegisterShafts(mine);

            return mine;
        }
    }
}
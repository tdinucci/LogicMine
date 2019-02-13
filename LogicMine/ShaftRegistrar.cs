namespace LogicMine
{
    /// <inheritdoc />
    public abstract class ShaftRegistrar : IShaftRegistrar
    {
        /// <inheritdoc />
        public abstract void RegisterShafts(IMine mine);
    }
}
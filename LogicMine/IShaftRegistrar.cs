namespace LogicMine
{
    /// <summary>
    /// Represents a type that can register shafts within a mine
    /// </summary>
    public interface IShaftRegistrar
    {
        /// <summary>
        /// Registers a set of shafts within a mine
        /// </summary>
        /// <param name="mine">The mine to register shafts with</param>
        void RegisterShafts(IMine mine);
    }
}
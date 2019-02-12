namespace LogicMine
{
    /// <summary>
    /// A type that can extract the trace information from an <see cref="IBasket"/> and export it somewhere
    /// </summary>
    public interface ITraceExporter : IErrorExporter
    {
        /// <summary>
        /// Exports the baskets trace information
        /// </summary>
        /// <param name="basket">The basket to export trace information for</param>
        void Export(IBasket basket);
    }
}
namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// A type which contains metadata related to objects which are mapped to a database table.
    /// </summary>
    public interface IMappedDataObjectDescriptor : IDataObjectDescriptor
    {
        /// <summary>
        /// The schema which the mapped table is within
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// The name of the mapped table
        /// </summary>
        string Table { get; }

        /// <summary>
        /// The name of the primary key within the table - this is expected to be the records identity
        /// </summary>
        string PrimaryKey { get; }

        /// <summary>
        /// The complete name of the table, e.g. dbo.Frog
        /// </summary>
        string FullTableName { get; }
    }
}
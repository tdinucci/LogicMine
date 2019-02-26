namespace LogicMine.DataObject.Ado.PostgreSql
{
    /// <summary>
    /// A type which contains metadata related to objects which are stored in a Postgres database.
    /// </summary>
    /// <typeparam name="T">The type described</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class PostgreSqlObjectDescriptor<T, TId> : DataObjectDescriptor<T, TId>
    {
        /// <summary>
        /// Construct a new PostgreSqlObjectDescriptor
        /// </summary>
        /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
        public PostgreSqlObjectDescriptor(params string[] readOnlyPropertyNames) : base(readOnlyPropertyNames)
        {
        }
    }
}
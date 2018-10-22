using System.Collections.Generic;

namespace LogicMine.Api.Data.Postgres
{
    /// <summary>
    /// A type which contains metadata related to objects which are stored in a Postgres database.
    /// </summary>
    /// <typeparam name="T">The type described</typeparam>
    public class PostgresObjectDescriptor<T> : DbObjectDescriptor<T>
    {
        /// <summary>
        /// Construct a new PostgresObjectDescriptor
        /// </summary>
        public PostgresObjectDescriptor() : base()
        {
        }

        /// <summary>
        /// Construct a new PostgresObjectDescriptor
        /// </summary>
        /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
        public PostgresObjectDescriptor(IEnumerable<string> readOnlyPropertyNames) : base(readOnlyPropertyNames)
        {
        }
    }
}
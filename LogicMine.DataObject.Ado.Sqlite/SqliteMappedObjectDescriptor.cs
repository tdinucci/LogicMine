using System;

namespace LogicMine.DataObject.Ado.Sqlite
{
    /// <summary>
    /// A type which contains metadata related to objects which are mapped to SQLite tables.
    /// </summary>
    /// <typeparam name="T">The type described</typeparam>
    /// <typeparam name="TId">The identity type on T</typeparam>
    public class SqliteMappedObjectDescriptor<T, TId> : SqliteObjectDescriptor<T, TId>, IMappedDataObjectDescriptor
    {
        /// <summary>
        /// The schema of the table which is mapped to type T.  By default this is "main"
        /// </summary>
        public virtual string Schema => "main";

        /// <inheritdoc />
        public virtual string Table { get; }

        /// <inheritdoc />
        public virtual string PrimaryKey { get; }

        /// <inheritdoc />
        public string FullTableName => $"{Schema}.{Table}";

        /// <summary>
        /// Construct a new SqliteMappedObjectDescriptor
        /// </summary>
        /// <param name="table">The table T is mapped to</param>
        /// <param name="primaryKey">The primary key on the table T is mapped to</param>
        public SqliteMappedObjectDescriptor(string table, string primaryKey) : this(table, primaryKey, null)
        {
        }

        /// <summary>
        /// Construct a new SqliteMappedObjectDescriptor
        /// </summary>
        /// <param name="table">The table T is mapped to</param>
        /// <param name="primaryKey">The primary key on the table T is mapped to</param>
        /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
        public SqliteMappedObjectDescriptor(string table, string primaryKey, params string[] readOnlyPropertyNames) :
            base(readOnlyPropertyNames)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(primaryKey))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(primaryKey));

            Table = table;
            PrimaryKey = primaryKey;
        }
    }
}
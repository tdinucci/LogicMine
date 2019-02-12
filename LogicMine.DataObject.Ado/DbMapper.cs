using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FastMember;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// A basic object-relational mapper
    /// </summary>
    /// <typeparam name="T">The type which the mapper operates on</typeparam>
    public class DbMapper<T> : IDbMapper<T> where T : new()
    {
        /// <summary>
        /// Contains metadata to enable mapping T's to database tables
        /// </summary>
        protected IDataObjectDescriptor Descriptor { get; }

        /// <summary>
        /// Construct a new DbMapper
        /// </summary>
        /// <param name="descriptor">Metadata to enable mapping T's to database tables</param>
        public DbMapper(IDataObjectDescriptor descriptor)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        /// <inheritdoc />
        public virtual T MapObject(IDataRecord record)
        {
            var result = new T();
            var accessor = TypeAccessor.Create(typeof(T));

            foreach (var member in accessor.GetMembers().Where(m => m.CanWrite && Descriptor.CanRead(m.Name)))
            {
                try
                {
                    var columnName = Descriptor.GetMappedColumnName(member.Name);
                    if (!string.IsNullOrWhiteSpace(columnName))
                    {
                        // TODO: Use a reference to an ordinals dictionary so that if this 
                        // method is called in a loop it doesn't have to repeatedly work this out.
                        var ordinal = record.GetOrdinal(columnName);
                        if (ordinal >= 0)
                        {
                            var value = record[ordinal];
                            value = value == DBNull.Value ? null : Descriptor.ProjectColumnValue(value, member.Type);

                            accessor[result, member.Name] = value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to map property '{member.Name}'", ex);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public virtual T[] MapObjects(IDataReader reader)
        {
            var result = new List<T>();
            while (reader.Read())
                result.Add(MapObject(reader));

            return result.ToArray();
        }
    }
}
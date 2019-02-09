using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogicMine.DataObject
{
    public class DataObjectDescriptor<T, TId> : DataObjectDescriptor
    {
        protected DataObjectDescriptor(params string[] readOnlyPropertyNames) :
            base(typeof(T), typeof(TId), readOnlyPropertyNames)
        {
        }
    }

    /// <summary>
    /// A type which contains metadata related to objects which are stored in a database.
    /// </summary>
    public class DataObjectDescriptor : IDataObjectDescriptor
    {
        private static IEnumerable<PropertyInfo> _readablePropertyNames;

        /// <inheritdoc />
        public Type DataType { get; }

        /// <inheritdoc />
        public Type IdType { get; }

        /// <summary>
        /// A collection of property names which should not be written to the database
        /// </summary>
        protected HashSet<string> ReadOnlyPropertyNames { get; }

        /// <summary>
        /// Construct a new DbObjectDescriptor
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="idType"></param>
        /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
        protected DataObjectDescriptor(Type dataType, Type idType, params string[] readOnlyPropertyNames)
        {
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
            IdType = idType ?? throw new ArgumentNullException(nameof(idType));
            ReadOnlyPropertyNames = new HashSet<string>(readOnlyPropertyNames ?? new string[0]);
        }

        /// <inheritdoc />
        public virtual bool CanWrite(string propertyName)
        {
            if (ReadOnlyPropertyNames != null)
                return !ReadOnlyPropertyNames.Contains(propertyName);

            return true;
        }

        /// <inheritdoc />
        public virtual bool CanRead(string propertyName)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual string GetMappedColumnName(string propertyName)
        {
            return propertyName;
        }

        /// <inheritdoc />
        public virtual object ProjectColumnValue(object columnValue, Type propertyType)
        {
            return columnValue;
        }

        /// <inheritdoc />
        public TProjected ProjectColumnValue<TProjected>(object columnValue)
        {
            return (TProjected) ProjectColumnValue(columnValue, typeof(TProjected));
        }

        /// <inheritdoc />
        public virtual object ProjectPropertyValue(object propertyValue, string propertyName)
        {
            return propertyValue;
        }

        public IEnumerable<PropertyInfo> GetReadableProperties()
        {
            if (_readablePropertyNames == null)
                _readablePropertyNames = DataType.GetProperties().Where(p => CanRead(p.Name));

            return _readablePropertyNames;
        }
    }
}
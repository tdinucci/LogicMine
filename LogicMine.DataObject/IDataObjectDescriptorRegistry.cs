using System;
using System.Collections.Generic;

namespace LogicMine.DataObject
{
    /// <summary>
    /// A registry for data object descriptors.  
    /// </summary>
    public interface IDataObjectDescriptorRegistry
    {
        /// <summary>
        /// Registers a descriptor in the registry
        /// </summary>
        /// <param name="descriptor">The descriptor to register</param>
        /// <returns>A reference to "this" registry</returns>
        IDataObjectDescriptorRegistry Register(IDataObjectDescriptor descriptor);

        /// <summary>
        /// Get a descriptor for a particular data type
        /// </summary>
        /// <param name="dataType">The type the descriptor is required for</param>
        /// <returns>The relevant descriptor</returns>
        IDataObjectDescriptor GetDescriptor(Type dataType);
        
        /// <summary>
        /// Get a descriptor for a particular data type - when given the data type name
        /// </summary>
        /// <param name="dataTypeName">The name of the type the descriptor is required for</param>
        /// <returns>The relevant descriptor</returns>
        IDataObjectDescriptor GetDescriptor(string dataTypeName);
        
        /// <summary>
        /// Get a type safe descriptor for a particular data type
        /// </summary>
        /// <typeparam name="T">The type the descriptor is needed for</typeparam>
        /// <typeparam name="TDescriptor">The type of the descriptor required</typeparam>
        /// <returns></returns>
        TDescriptor GetDescriptor<T, TDescriptor>() where TDescriptor : IDataObjectDescriptor;

        /// <summary>
        /// Get the collection of data types which descriptors have been registered for 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetKnownDataTypes();
    }
}
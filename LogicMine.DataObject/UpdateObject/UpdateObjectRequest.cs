using System;
using System.Collections.Generic;

namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectRequest<T, TId> : Request
    {
        public Type DataType { get; } = typeof(T);
        public TId Id { get; }
        public IDictionary<string, object> ModifiedProperties { get; }


        public UpdateObjectRequest(TId id, IDictionary<string, object> modifiedProperties)
        {
            if (modifiedProperties == null || modifiedProperties.Count == 0)
                throw new ArgumentException($"'{nameof(modifiedProperties)}' must have a value");

            Id = id;
            ModifiedProperties = modifiedProperties;
        }
    }
}
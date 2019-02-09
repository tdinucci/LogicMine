using System;
using System.Collections.Generic;

namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectRequest<T, TId> : Request
    {
        public Type ObjectType { get; } = typeof(T);
        public TId ObjectId { get; }
        public IDictionary<string, object> ModifiedProperties { get; }


        public UpdateObjectRequest(TId objectId, IDictionary<string, object> modifiedProperties)
        {
            if (modifiedProperties == null || modifiedProperties.Count == 0)
                throw new ArgumentException($"'{nameof(modifiedProperties)}' must have a value");

            ObjectId = objectId;
            ModifiedProperties = modifiedProperties;
        }
    }
}
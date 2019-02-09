using System;

namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectResponse : Response
    {
        public bool Success { get; }

        public UpdateObjectResponse()
        {
        }

        public UpdateObjectResponse(Guid requestId, bool success, string error = null) : base(requestId, error)
        {
            Success = success;
        }
    }
}
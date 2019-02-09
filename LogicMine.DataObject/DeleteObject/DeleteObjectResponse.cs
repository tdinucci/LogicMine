using System;

namespace LogicMine.DataObject.DeleteObject
{
    public class DeleteObjectResponse : Response
    {
        public bool Success { get; }

        public DeleteObjectResponse()
        {
        }
        
        public DeleteObjectResponse(Guid requestId, bool success, string error = null) : base(requestId, error)
        {
            Success = success;
        }
    }
}
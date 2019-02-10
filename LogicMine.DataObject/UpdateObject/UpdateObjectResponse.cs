using System;

namespace LogicMine.DataObject.UpdateObject
{
    public class UpdateObjectResponse : Response
    {
        public bool Success { get; }

        public UpdateObjectResponse(IRequest request) : base(request)
        {
        }

        public UpdateObjectResponse(IRequest request, bool success, string error = null) : base(request, error)
        {
            Success = success;
        }
    }
}
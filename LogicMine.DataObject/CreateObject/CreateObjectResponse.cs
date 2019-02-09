using System;

namespace LogicMine.DataObject.CreateObject
{
    public class CreateObjectResponse<TId> : Response
    {
        public TId ObjectId { get; }

        public CreateObjectResponse()
        {
        }

        public CreateObjectResponse(Guid requestId, TId objectId) : base(requestId)
        {
            ObjectId = objectId;
        }
    }
}
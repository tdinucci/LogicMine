using System.Collections.Generic;

namespace LogicMine.Api.Web.Messaging.Request
{
    public class CreateObjectRequest : ObjectRequest
    {
        public Dictionary<string, object> Object { get; set; }
    }
}
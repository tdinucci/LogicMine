using System.Collections.Generic;

namespace LogicMine.Api.Web.Messaging.Request
{
    public class PatchObjectRequest : ObjectRequest
    {
        public string Id { get; set; }
        public Dictionary<string, object> ModifiedProperties { get; set; }
    }
}
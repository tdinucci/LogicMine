namespace LogicMine.Api.Web.Messaging.Request
{
    public class GetCollectionObjectRequest : ObjectRequest
    {
        public int? Max { get; set; }
        public int? Page { get; set; }
        public string Filter { get; set; }
    }
}
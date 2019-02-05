namespace LogicMine.Api.Web.Messaging.Response
{
    public class ObjectResponse : IResponse
    {
        public string Type { get; set; }
        public string Result { get; set; }
    }
}
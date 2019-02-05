namespace LogicMine.Api.Web.Messaging.Response
{
    public class ErrorResponse : IResponse
    {
        public string ErrorType { get; set; }
        public string Message { get; set; }
    }
}
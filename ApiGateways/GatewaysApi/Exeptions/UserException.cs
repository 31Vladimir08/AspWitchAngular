using System.Net;

namespace GatewaysApi.Exeptions
{
    public class UserException : Exception
    {
        public UserException(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public HttpStatusCode StatusCode { get; }
        public override string Message { get; }
    }
}

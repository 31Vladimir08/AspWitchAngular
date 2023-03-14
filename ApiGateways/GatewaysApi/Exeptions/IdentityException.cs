using System.Net;

namespace GatewaysApi.Exeptions
{
    public class IdentityException : Exception
    {
        public IdentityException(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public HttpStatusCode StatusCode { get; }
        public override string Message { get; }
    }
}

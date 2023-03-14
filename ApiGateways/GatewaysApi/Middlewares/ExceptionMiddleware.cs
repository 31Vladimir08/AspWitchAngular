using System.Net;
using System.Text.Json;
using GatewaysApi.Exeptions;

namespace GatewaysApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (IdentityException e)
            {
                _logger.LogError(e.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)e.StatusCode;
                await context.Response.WriteAsync(e.Message);
            }
            catch (UserException e)
            {
                _logger.LogError(e.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)e.StatusCode;
                await context.Response.WriteAsync(GetMessageErrors(e));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(HttpStatusCode.NotFound.ToString());
            }
        }
        
        private string GetMessageErrors(UserException exception)
        {
            return JsonSerializer.Serialize(new
            {
                errors = new List<object>()
                {
                    new
                    {
                        code = $"Error: {(int)exception.StatusCode}",
                        description = exception.Message
                    }
                }
            });
        }
    }
}

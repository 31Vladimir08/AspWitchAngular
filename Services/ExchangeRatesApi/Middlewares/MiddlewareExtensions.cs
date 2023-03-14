using Microsoft.AspNetCore.Builder;

namespace ExchangeRatesApi.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}

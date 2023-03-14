namespace GatewaysApi.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}

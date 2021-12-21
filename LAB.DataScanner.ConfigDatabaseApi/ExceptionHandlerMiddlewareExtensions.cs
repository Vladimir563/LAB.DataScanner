using Microsoft.AspNetCore.Builder;

namespace LAB.DataScanner.ConfigDatabaseApi
{
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }

    }
}

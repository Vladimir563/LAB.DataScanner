using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LAB.DataScanner.ConfigDatabaseApi
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

    }
}

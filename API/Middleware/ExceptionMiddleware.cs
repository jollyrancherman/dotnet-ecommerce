using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private readonly JsonSerializerOptions _serializerOptions;

        public ExceptionMiddleware
        (
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger, 
            IHostEnvironment env
        )
        {
            _logger = logger;
            _env = env;
            _next = next;
            _serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // if no exception, continue
            }
            catch (Exception ex)
            {
                // log exception to terminal and to file (if in production)
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                // if in development, return stack trace
                var response = _env.IsDevelopment()
                    ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ApiException((int)HttpStatusCode.InternalServerError);


                // return response as json
                var json = JsonSerializer.Serialize(response, _serializerOptions);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
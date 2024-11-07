using Microsoft.Extensions.Logging;

namespace AuthorsWebApi.Middlewares
{
    public static class LogHttpResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogHttpResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogHttpResponseMiddleware>();
        }
    }

    public class LogHttpResponseMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogHttpResponseMiddleware> logger;

        public LogHttpResponseMiddleware(RequestDelegate next, ILogger<LogHttpResponseMiddleware> logger) {
            this.next = next;
            this.logger = logger;
        }

        //Invoke or InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalResponseBody = context.Response.Body;
                context.Response.Body = ms;
                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;
                logger.LogInformation(response);
            }
        }
    }
}

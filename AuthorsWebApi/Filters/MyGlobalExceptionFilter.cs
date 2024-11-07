using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthorsWebApi.Filters
{
    public class MyGlobalExceptionFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<MyGlobalExceptionFilter> logger;

        public MyGlobalExceptionFilter(ILogger<MyGlobalExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError("This is a custom exception message");
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}

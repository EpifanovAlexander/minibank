using Minibank.Core.Exceptions;

namespace Minibank.Web.Middlewares
{
    public class UserFriendlyExceptionMiddleware
    {
        public readonly RequestDelegate next;

        public UserFriendlyExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (UserFriendlyException exception)
            {
                await httpContext.Response.WriteAsJsonAsync(new { Message = exception.Message });
            }
        }
    }
}

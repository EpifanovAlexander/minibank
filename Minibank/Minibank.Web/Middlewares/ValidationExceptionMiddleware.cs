using Minibank.Core.Exceptions;

namespace Minibank.Web.Middlewares
{
    public class ValidationExceptionMiddleware
    {
        public readonly RequestDelegate next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (FluentValidation.ValidationException exception)
            {
                var errors = exception.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                var errorMessage = string.Join(Environment.NewLine, errors);

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { errorMessage });
            }
            catch (ValidationException exception)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { exception.Message });
            }
        }
    }
}

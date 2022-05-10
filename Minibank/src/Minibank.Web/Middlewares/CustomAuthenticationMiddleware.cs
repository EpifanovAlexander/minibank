using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Minibank.Web.Middlewares
{
    public class CustomAuthenticationMiddleware
    {
        public readonly RequestDelegate next;

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                var now = DateTimeOffset.Now;

                string authorization = httpContext.Request.Headers[HeaderNames.Authorization];
                if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                {
                    throw new UnauthorizedAccessException();
                }

                var token = authorization.Substring("Bearer ".Length).Trim();
                var payload = token.Split('.')[1];

                StringBuilder decodedPayload = new StringBuilder("");
                foreach (var code in WebEncoders.Base64UrlDecode(payload))
                {
                    decodedPayload.Append((char)code);
                }

                var exp = decodedPayload.ToString()
                    .Split(',')
                    .FirstOrDefault(c => c.StartsWith("\"exp\""));

                if (exp == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var time = Convert.ToInt32(exp.Substring(exp.IndexOf(":") + 1));

                var normalDateTime = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(time);

                var nowUnixTime = now.ToUnixTimeSeconds();

                if (time < nowUnixTime)
                {
                    throw new SecurityTokenExpiredException();
                }

                await next(httpContext);
            }
            catch (UnauthorizedAccessException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            catch (SecurityTokenExpiredException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsJsonAsync(new { Message = "JWT токен просрочен" });
            }
        }
    }

}

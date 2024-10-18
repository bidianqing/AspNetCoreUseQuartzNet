using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace AspNetCoreUseQuartzNet
{
    public class QuartzUIAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public QuartzUIAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/quartz", StringComparison.OrdinalIgnoreCase))
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"BasicAuthentication\", charset=\"UTF-8\"";
                    context.Response.StatusCode = 401;
                    return;
                }

                try
                {
                    if (!AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out var authHeader))
                    {
                        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"BasicAuthentication\", charset=\"UTF-8\"";
                        context.Response.StatusCode = 401;
                        return;
                    }

                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                    if (credentials.Length != 2)
                    {
                        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"BasicAuthentication\", charset=\"UTF-8\"";
                        context.Response.StatusCode = 401;
                        return;
                    }
                    var username = credentials[0];
                    var password = credentials[1];
                    if (username != "admin" || password != "123456")
                    {
                        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"BasicAuthentication\", charset=\"UTF-8\"";
                        context.Response.StatusCode = 401;
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"BasicAuthentication\", charset=\"UTF-8\"";
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class QuartzUIAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseQuartzUIAuthentication(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<QuartzUIAuthenticationMiddleware>();
        }
    }
}

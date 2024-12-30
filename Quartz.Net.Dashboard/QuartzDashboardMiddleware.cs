using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Net.Dashboard
{
    public class QuartzDashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IScheduler _scheduler;

        public QuartzDashboardMiddleware(RequestDelegate next, IScheduler scheduler)
        {
            _next = next;
            _scheduler = scheduler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/quartz-dashboard", StringComparison.OrdinalIgnoreCase))
            {
                // 页面路由读取对应html源文件
                // 接口路由执行对应的命令
                using (var inputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Quartz.Net.Dashboard.Pages.jobs.html"))
                {
                    if (inputStream != null)
                    {
                        context.Response.ContentType = MediaTypeNames.Text.Html;
                        await inputStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
                    }
                }

                return;
            }

            await _next(context);
        }
    }

    public static class QuartzDashboardMiddlewareExtensions
    {
        public static IApplicationBuilder UseQuartzDashboard(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<QuartzDashboardMiddleware>();
        }
    }
}

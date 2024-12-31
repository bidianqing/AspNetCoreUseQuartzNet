using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Quartz.Net.Dashboard.Commands;
using Quartz.Util;
using System.Net.Mime;
using System.Reflection;
using static Quartz.SchedulerBuilder;

namespace Quartz.Net.Dashboard
{
    public class QuartzDashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IDbConnectionManager _dbConnectionManager;

        public QuartzDashboardMiddleware(RequestDelegate next, ISchedulerFactory schedulerFactory, IDbConnectionManager dbConnectionManager)
        {
            _next = next;
            _schedulerFactory = schedulerFactory;
            _dbConnectionManager = dbConnectionManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var connection = _dbConnectionManager.GetConnection(AdoProviderOptions.DefaultDataSourceName);
            if (context.Request.Path.StartsWithSegments("/quartz-dashboard", StringComparison.OrdinalIgnoreCase))
            {
                // 页面路由读取对应html源文件
                // 接口路由执行对应的命令
                using (var inputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Quartz.Net.Dashboard.index.html"))
                {
                    if (inputStream != null)
                    {
                        context.Response.ContentType = MediaTypeNames.Text.Html;
                        await inputStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);

                        await new JobListCommand(_schedulerFactory).ExecuteAsync();
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
            this IApplicationBuilder builder, ISchedulerFactory schedulerFactory)
        {
            return builder.UseMiddleware<QuartzDashboardMiddleware>(schedulerFactory);
        }
    }
}

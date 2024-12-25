using Quartz;
using System.ComponentModel;

namespace AspNetCoreUseQuartzNet.Jobs
{
    [Description("ContinueJob")]
    public class ContinueJob : IJob
    {
        private readonly ILogger<ContinueJob> _logger;

        public ContinueJob(ILogger<ContinueJob> logger)
        {
            _logger = logger;
        }

        public static readonly JobKey Key = new JobKey("continue", "default");

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("ContinueJob 执行");

            await Task.CompletedTask;
        }
    }
}

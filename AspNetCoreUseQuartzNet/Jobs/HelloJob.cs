﻿using Quartz;

namespace AspNetCoreUseQuartzNet.Jobs
{
    public class HelloJob : IJob
    {
        private readonly ILogger<HelloJob> _logger;

        public HelloJob(ILogger<HelloJob> logger, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// https://www.quartz-scheduler.net/documentation/best-practices.html#static-job-key
        /// </summary>
        public static readonly JobKey Key = new JobKey("hello", "default");

        public async Task Execute(IJobExecutionContext context)
        {
            // https://www.quartz-scheduler.net/documentation/best-practices.html#only-store-primitive-data-types-including-strings-in-the-jobdatamap
            // https://www.quartz-scheduler.net/documentation/best-practices.html#use-the-merged-jobdatamap
            context.MergedJobDataMap.TryGetString("name", out string name);

            _logger.LogInformation($"{DateTime.Now} ： hello {name}");
            // 业务逻辑处理



            // 后续Trigger
            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(ContinueJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(DateTime.Now.AddSeconds(3))
                        .Build();

            await context.Scheduler.ScheduleJob(trigger);
        }
    }
}

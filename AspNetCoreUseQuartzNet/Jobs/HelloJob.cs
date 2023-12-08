using Quartz;

namespace AspNetCoreUseQuartzNet.Jobs
{
    public class HelloJob : IJob
    {
        /// <summary>
        /// https://www.quartz-scheduler.net/documentation/best-practices.html#static-job-key
        /// </summary>
        public static readonly JobKey Key = new JobKey("hello", "default");

        public async Task Execute(IJobExecutionContext context)
        {
            // https://www.quartz-scheduler.net/documentation/best-practices.html#only-store-primitive-data-types-including-strings-in-the-jobdatamap
            // https://www.quartz-scheduler.net/documentation/best-practices.html#use-the-merged-jobdatamap
            context.MergedJobDataMap.TryGetString("name", out string name);

            await Console.Out.WriteLineAsync($"{DateTime.Now} ： hello {name}");
        }
    }
}

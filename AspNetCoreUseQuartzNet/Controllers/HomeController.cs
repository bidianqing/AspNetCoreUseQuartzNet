using AspNetCoreUseQuartzNet.Jobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace AspNetCoreUseQuartzNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<HomeController> _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public HomeController(ILogger<HomeController> logger, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get([FromQuery] string name)
        {
            // ָ��ʱ��ִ�е�һ�Σ�Ȼ��ÿ��3����ִ��һ��

            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(HelloJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(DateTime.Now.AddSeconds(10))
                        .WithSimpleSchedule(x => x.WithIntervalInMinutes(3).RepeatForever())
                        .Build();
            //*/


            // ָ��ʱ��ִ�е�һ�Σ�Ȼ��ÿ��2Сʱִ��һ��
            /*
            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(HelloJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(DateTime.Now.AddSeconds(10))
                        .WithSimpleSchedule(x => x.WithIntervalInHours(2).RepeatForever())
                        .Build();
            //*/


            // ָ��ʱ��ִ�е�һ�Σ�Ȼ��ÿ��2��ִ��һ��
            /*
            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(HelloJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(DateTime.Now.AddSeconds(10))
                        .WithCalendarIntervalSchedule(x => x.WithIntervalInDays(2))
                        .Build();
            //*/


            // ָ��ʱ��ִ�е�һ�Σ�Ȼ��ÿ��2��ִ��һ��
            /*
            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(HelloJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(DateTime.Now.AddSeconds(10))
                        .WithCalendarIntervalSchedule(x => x.WithIntervalInWeeks(2))
                        .Build();
            //*/


            // ָ��ʱ��ִ�е�һ�Σ�Ȼ��ÿ��2��ִ��һ��
            /*
            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(HelloJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(DateTime.Now.AddSeconds(10))
                        .WithCalendarIntervalSchedule(x => x.WithIntervalInMonths(2))
                        .Build();
            //*/

            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.ScheduleJob(trigger);

            // ����ִ��һ��Job
            //await scheduler.TriggerJob(HelloJob.Key);

            // ֹͣ����ĳ��������
            //await scheduler.UnscheduleJob(trigger.Key);


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

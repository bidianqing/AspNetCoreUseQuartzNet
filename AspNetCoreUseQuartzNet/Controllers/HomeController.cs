using AspNetCoreUseQuartzNet.Jobs;
using CrystalQuartz.Core.Domain.Activities;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Simpl;
using Quartz.Util;
using System.Collections.ObjectModel;

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
            // ָ��ʱ��ִ�е�һ�Σ�Ȼ��ÿ��1����ִ��һ��

            ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(HelloJob.Key)
                        .UsingJobData("name", name)
                        .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(3)))
                        //.WithCalendarIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second).WithMisfireHandlingInstructionDoNothing())
                        //.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                        .WithCronSchedule("0 0/5 * * * ?")
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

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// ��ͣĳ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("PauseTrigger")]
        public async Task PauseTrigger([FromQuery] string name)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            await scheduler.PauseTrigger(new TriggerKey(name));
        }

        /// <summary>
        /// �ָ�ĳ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("ResumeTrigger")]
        public async Task ResumeTrigger([FromQuery] string name)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            await scheduler.ResumeTrigger(new TriggerKey(name));
        }

        /// <summary>
        /// ж��ĳ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("UnscheduleJob")]
        public async Task UnscheduleJob([FromQuery] string name)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            await scheduler.UnscheduleJob(new TriggerKey(name));
        }

        /// <summary>
        /// ��ȡ���д�����
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllTrigger")]
        public async Task<List<TriggerKey>> AllTrigger()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var triggers = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());

            return triggers.ToList();
        }

        /// <summary>
        /// ɾ�����д�����
        /// </summary>
        /// <returns></returns>
        [HttpGet("DeleteAllTrigger")]
        public async Task<string> DeleteAllTrigger()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var triggers = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());

            foreach (var trigger in triggers)
            {
                await scheduler.UnscheduleJob(new TriggerKey(trigger.Name, trigger.Group));
            }

            return "�����ɹ�";
        }

        /// <summary>
        /// ����ִ��һ��Job
        /// </summary>
        /// <returns></returns>
        [HttpGet("TriggerJob")]
        public async Task TriggerJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var data = new JobDataMap();
            data.PutAll(new Dictionary<string, object> { { "name", "bi" } });
            await scheduler.TriggerJob(HelloJob.Key, data);
        }

        /// <summary>
        /// ��ȡ���������´�ִ��ʱ��
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNextFireTimeUtc")]
        public async Task<DateTimeOffset?> GetNextFireTimeUtc()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            ITrigger trigger = await scheduler.GetTrigger(new TriggerKey("facda4bf-302c-44e3-9c6c-c761abd2c7ac"));


            var nextTime = trigger.GetNextFireTimeUtc();

            return nextTime;
        }
    }
}

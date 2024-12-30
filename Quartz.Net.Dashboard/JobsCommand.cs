using Quartz.Impl;
using Quartz.Util;
using System.Reflection;
using static Quartz.SchedulerBuilder;

namespace Quartz.Net.Dashboard
{
    public class JobsCommand
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public JobsCommand(ISchedulerFactory schedulerFactory) 
        {
            _schedulerFactory = schedulerFactory;
        }

        public async void ExecuteAsync()
        {
            var stdSchedulerFactory = _schedulerFactory as StdSchedulerFactory;
            var methedInfo = stdSchedulerFactory.GetType().GetMethod("GetDBConnectionManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var dbConnectionManager = methedInfo.Invoke(stdSchedulerFactory, null) as IDbConnectionManager;
            var dbConnection = dbConnectionManager.GetConnection(AdoProviderOptions.DefaultDataSourceName);

            var scheduler = await _schedulerFactory.GetScheduler();
        }
    }
}

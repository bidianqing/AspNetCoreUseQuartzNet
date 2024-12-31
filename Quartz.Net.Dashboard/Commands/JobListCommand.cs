using Quartz.Impl;
using Quartz.Util;
using System.Reflection;
using static Quartz.SchedulerBuilder;

namespace Quartz.Net.Dashboard.Commands
{
    public class JobListCommand
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public JobListCommand(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task ExecuteAsync()
        {
            var stdSchedulerFactory = _schedulerFactory as StdSchedulerFactory;
            var methedInfo = stdSchedulerFactory.GetType().GetMethod("GetDBConnectionManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var dbConnectionManager = methedInfo.Invoke(stdSchedulerFactory, null) as IDbConnectionManager;
            var dbConnection = dbConnectionManager.GetConnection(AdoProviderOptions.DefaultDataSourceName);
            var dbProvider = dbConnectionManager.GetDbProvider(AdoProviderOptions.DefaultDataSourceName);

            var scheduler = await _schedulerFactory.GetScheduler();
        }
    }
}

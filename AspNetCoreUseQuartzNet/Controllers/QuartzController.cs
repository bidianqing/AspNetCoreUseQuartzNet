using AspNetCoreUseQuartzNet.Tables;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Util;
using System.Text;
using System.Text.Json.Nodes;
using static Quartz.SchedulerBuilder;

namespace AspNetCoreUseQuartzNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuartzController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IDbConnectionManager _connectionManager;
        public QuartzController(ISchedulerFactory schedulerFactory, IDbConnectionManager connectionManager)
        {
            _schedulerFactory = schedulerFactory;
            _connectionManager = connectionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs")]
        public async Task<IEnumerable<qrtz_job_details>> GetJobs([FromQuery] string jobName, [FromQuery] string jobGroup)
        {
            StringBuilder where = new StringBuilder(" 1 = 1 ");
            DynamicParameters parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(jobName))
            {
                where.Append($" and JOB_NAME = @JOBNAME");
                parameters.Add("JOBNAME", jobName);
            }

            if (!string.IsNullOrEmpty(jobGroup))
            {
                where.Append($" and JOB_GROUP = @JOBGROUP");
                parameters.Add("JOBGROUP", jobGroup);
            }
            var connection = _connectionManager.GetConnection(AdoProviderOptions.DefaultDataSourceName);
            return await connection.QueryAsync<qrtz_job_details>($"select * from qrtz_job_details where {where}", parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("triggerJob")]
        public async Task TriggerJob([FromBody] JsonObject obj)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var data = new JobDataMap();

            obj["variables"].AsArray().ToList().ForEach(u => data.Put(u["variableName"].ToString(), u["variableValue"].ToString()));

            await scheduler.TriggerJob(new JobKey(obj["jobName"].ToString(), obj["jobGroup"].ToString()), data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("triggers")]
        public async Task<IEnumerable<qrtz_triggers>> GetTriggers([FromQuery] string jobName, [FromQuery] string jobGroup)
        {
            StringBuilder where = new StringBuilder(" 1 = 1 ");
            DynamicParameters parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(jobName))
            {
                where.Append($" and JOB_NAME = @JOBNAME");
                parameters.Add("JOBNAME", jobName);
            }

            if (!string.IsNullOrEmpty(jobGroup))
            {
                where.Append($" and JOB_GROUP = @JOBGROUP");
                parameters.Add("JOBGROUP", jobGroup);
            }
            var connection = _connectionManager.GetConnection(AdoProviderOptions.DefaultDataSourceName);
            return await connection.QueryAsync<qrtz_triggers>($"select * from qrtz_triggers where {where}", parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("triggerAction")]
        public async Task TriggerAction([FromBody] JsonObject obj)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var command = obj["command"].ToString();
            var triggerName = obj["triggerName"].ToString();
            var triggerGroup = obj["triggerGroup"].ToString();
            var triggerKey = new TriggerKey(triggerName, triggerGroup);

            if (command == "pause")
            {
                await scheduler.PauseTrigger(triggerKey);
            }
            else if(command == "resume")
            {
                await scheduler.ResumeTrigger(triggerKey);
            }
            else if(command == "unschedule")
            {
                await scheduler.UnscheduleJob(triggerKey);
            }
        }
    }
}

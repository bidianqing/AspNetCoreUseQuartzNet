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
        public async Task<IEnumerable<qrtz_job_details>> GetJobs([FromQuery] string JOB_NAME, [FromQuery] string JOB_GROUP)
        {
            StringBuilder where = new StringBuilder(" 1 = 1 ");
            DynamicParameters parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(JOB_NAME))
            {
                where.Append($" and JOB_NAME = @JOBNAME");
                parameters.Add("JOBNAME", JOB_NAME);
            }

            if (!string.IsNullOrEmpty(JOB_GROUP))
            {
                where.Append($" and JOB_GROUP = @JOBGROUP");
                parameters.Add("JOBGROUP", JOB_GROUP);
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

            var jobKey = new JobKey(obj["JOB_NAME"].ToString(), obj["JOB_GROUP"].ToString());

            await scheduler.TriggerJob(jobKey, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("triggers")]
        public async Task<object> GetTriggers([FromQuery] string jobName, [FromQuery] string jobGroup, [FromQuery] string triggerName, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            StringBuilder where = new StringBuilder(" 1 = 1 ");
            DynamicParameters parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(triggerName))
            {
                where.Append($" and t1.TRIGGER_NAME = @TRIGGERNAME");
                parameters.Add("TRIGGERNAME", triggerName);
            }

            if (!string.IsNullOrEmpty(jobName))
            {
                where.Append($" and t1.JOB_NAME = @JOBNAME");
                parameters.Add("JOBNAME", jobName);
            }

            if (!string.IsNullOrEmpty(jobGroup))
            {
                where.Append($" and t1.JOB_GROUP = @JOBGROUP");
                parameters.Add("JOBGROUP", jobGroup);
            }

            string listSql = @$"select t1.*,t2.REPEAT_INTERVAL,t3.CRON_EXPRESSION,t4.STR_PROP_1,t4.INT_PROP_1
from qrtz_triggers t1
left join qrtz_simple_triggers t2 on t1.TRIGGER_NAME = t2.TRIGGER_NAME
left join qrtz_cron_triggers t3 on t1.TRIGGER_NAME = t3.TRIGGER_NAME
left join qrtz_simprop_triggers t4 on t1.TRIGGER_NAME = t4.TRIGGER_NAME
where {where}
limit {(pageIndex - 1) * pageSize},{pageSize}";
            string countSql = @$"select count(1) 
from qrtz_triggers t1
left join qrtz_simple_triggers t2 on t1.TRIGGER_NAME = t2.TRIGGER_NAME
left join qrtz_cron_triggers t3 on t1.TRIGGER_NAME = t3.TRIGGER_NAME
left join qrtz_simprop_triggers t4 on t1.TRIGGER_NAME = t4.TRIGGER_NAME
where {where}";
            var connection = _connectionManager.GetConnection(AdoProviderOptions.DefaultDataSourceName);
            var reader = await connection.QueryMultipleAsync($"{listSql};{countSql}", parameters);
            var triggers = await reader.ReadAsync<qrtz_triggers>();
            var total = await reader.ReadSingleAsync<int>();

            return new
            {
                list = triggers,
                total = total
            };
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

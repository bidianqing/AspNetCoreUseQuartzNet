using AspNetCoreUseQuartzNet.Tables;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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


        public async Task ExecuteJob([FromBody] JsonObject obj)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var data = new JobDataMap();

            string json = obj.ToJsonString();
            var jObject = JObject.Parse(json);
            var map = jObject.ToObject<Dictionary<string, object>>();
            data.PutAll(map);

            await scheduler.TriggerJob(new JobKey("", ""), data);
        }
    }
}

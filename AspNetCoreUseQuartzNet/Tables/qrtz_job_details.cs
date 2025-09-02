using System.Text.Json.Serialization;

namespace AspNetCoreUseQuartzNet.Tables
{
    public class qrtz_job_details
    {
        [JsonPropertyName(nameof(SCHED_NAME))]
        public string SCHED_NAME { get; set; }

        [JsonPropertyName(nameof(JOB_NAME))]
        public string JOB_NAME { get; set; }

        [JsonPropertyName(nameof(JOB_GROUP))]
        public string JOB_GROUP { get; set; }

        [JsonPropertyName(nameof(DESCRIPTION))]
        public string DESCRIPTION { get; set; }

        [JsonPropertyName(nameof(JOB_CLASS_NAME))]
        public string JOB_CLASS_NAME { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace AspNetCoreUseQuartzNet.Tables
{
    public class qrtz_triggers
    {
        [JsonPropertyName("SCHED_NAME")]
        public string SCHED_NAME { get; set; }

        [JsonPropertyName("TRIGGER_NAME")]
        public string TRIGGER_NAME { get; set; }

        [JsonPropertyName("TRIGGER_GROUP")]
        public string TRIGGER_GROUP { get; set; }

        [JsonPropertyName("JOB_NAME")]
        public string JOB_NAME { get; set; }

        [JsonPropertyName("JOB_GROUP")]
        public string JOB_GROUP { get; set; }

        [JsonPropertyName("TRIGGER_STATE")]
        public string TRIGGER_STATE { get; set; }

        [JsonPropertyName("START_TIME")]
        public long START_TIME { get; set; }

        public DateTimeOffset StartTime
        {
            get
            {
                return new DateTimeOffset(this.START_TIME, TimeSpan.Zero).ToLocalTime();
            }
        }
    }
}

using Quartz.Simpl;
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

        [JsonPropertyName("DESCRIPTION")]
        public string DESCRIPTION { get; set; }

        [JsonPropertyName("PREV_FIRE_TIME")]
        public long PREV_FIRE_TIME { get; set; }

        public DateTime PrevFireTime
        {
            get
            {
                return new DateTimeOffset(this.PREV_FIRE_TIME, TimeSpan.Zero).LocalDateTime;
            }
        }

        [JsonPropertyName("NEXT_FIRE_TIME")]
        public long NEXT_FIRE_TIME { get; set; }

        public DateTime NextFireTime
        {
            get
            {
                return new DateTimeOffset(this.NEXT_FIRE_TIME, TimeSpan.Zero).LocalDateTime;
            }
        }



        [JsonPropertyName("TRIGGER_STATE")]
        public string TRIGGER_STATE { get; set; }

        [JsonPropertyName("TRIGGER_TYPE")]
        public string TRIGGER_TYPE { get; set; }

        [JsonPropertyName("START_TIME")]
        public long START_TIME { get; set; }

        public DateTime StartTime
        {
            get
            {
                return new DateTimeOffset(this.START_TIME, TimeSpan.Zero).LocalDateTime;
            }
        }

        [JsonPropertyName("END_TIME")]
        public long? END_TIME { get; set; }

        public DateTime? EndTime
        {
            get
            {
                if (this.END_TIME == null) return null;
                return new DateTimeOffset(this.END_TIME.Value, TimeSpan.Zero).LocalDateTime;
            }
        }

        [JsonPropertyName("MISFIRE_INSTR")]
        public int MISFIRE_INSTR { get; set; }

        [JsonPropertyName("REPEAT_INTERVAL")]
        public long REPEAT_INTERVAL { get; set; }

        [JsonPropertyName("CRON_EXPRESSION")]
        public string CRON_EXPRESSION { get; set; }

        [JsonPropertyName("STR_PROP_1")]
        public string STR_PROP_1 { get; set; }

        [JsonPropertyName("INT_PROP_1")]
        public int INT_PROP_1 { get; set; }

        [JsonPropertyName("CALENDAR_INTERVAL")]
        public string CALENDAR_INTERVAL
        {
            get
            {
                return $"{this.INT_PROP_1}{this.STR_PROP_1}";
            }
        }

        [JsonPropertyName("JOB_DATA")]
        public byte[] JOB_DATA { get; set; }

        public Dictionary<string, object> JobData
        {
            get
            {
                try
                {
                    var serializer = new SystemTextJsonObjectSerializer();
                    serializer.Initialize();
                    return serializer.DeSerialize<Dictionary<string, object>>(this.JOB_DATA);
                }
                catch
                {
                    
                }

                return null;
            }
        }
    }
}

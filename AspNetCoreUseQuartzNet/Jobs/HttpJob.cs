using Quartz;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;

namespace AspNetCoreUseQuartzNet.Jobs
{
    [Description("HttpJob")]
    //[PersistJobDataAfterExecution]
    //[DisallowConcurrentExecution]
    public class HttpJob : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpJob(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public static readonly JobKey Key = new JobKey("http", "default");

        public async Task Execute(IJobExecutionContext context)
        {
            context.MergedJobDataMap.TryGetString("Method", out string method);
            context.MergedJobDataMap.TryGetString("Url", out string url);
            context.MergedJobDataMap.TryGetString("BasicUserName", out string basicUserName);
            context.MergedJobDataMap.TryGetString("BasicPassword", out string basicPassword);


            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            if (!string.IsNullOrWhiteSpace(basicUserName) && !string.IsNullOrWhiteSpace(basicPassword))
            {
                string parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicUserName}:{basicPassword}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", parameter);
            }

            await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}

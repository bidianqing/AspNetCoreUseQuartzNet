using Quartz;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace AspNetCoreUseQuartzNet.Jobs
{
    [Description("HttpJob")]
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
            context.MergedJobDataMap.TryGetString("method", out string httpMethod);
            context.MergedJobDataMap.TryGetString("url", out string requestUri);
            context.MergedJobDataMap.TryGetString("body", out string content);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), requestUri);
            if (!string.IsNullOrWhiteSpace(content))
            {
                request.Content = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            }

            var headers = Array.Empty<NameValueHeaderValue>();
            foreach (var item in headers)
            {
                request.Headers.Add(item.Name, item.Value);
            }

            await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}

using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tingle.Extensions.Logging.LogAnalytics;

namespace System.Net.Http
{
    internal static class HttpClientExtensions
    {
        private const string JsonContentType = "application/json";
        private const string DateHeaderName = "x-ms-date";


        public static async Task UploadAsync(this HttpClient httpClient,
                                             string workspaceId,
                                             byte[] workspaceKey,
                                             string payload,
                                             string logType,
                                             DateTimeOffset? generated = null)
        {
            try
            {
                await UploadInnerAsync(httpClient: httpClient,
                                       workspaceId: workspaceId,
                                       workspaceKey: workspaceKey,
                                       payload: payload,
                                       logType: logType,
                                       generated: generated);
            }
            catch (Exception ex)
            {
                LogAnalyticsLoggerEventSource.Log.FailedToLog(ex.ToInvariantString());
            }

        }

        public static async Task UploadInnerAsync(this HttpClient httpClient,
                                             string workspaceId,
                                             byte[] workspaceKey,
                                             string payload,
                                             string logType,
                                             DateTimeOffset? generated = null)
        {
            var encoding = Encoding.UTF8;

            var url = $"https://{workspaceId}.ods.opinsights.azure.com/api/logs?api-version=2016-04-01";
            var rfcDate = DateTimeOffset.UtcNow.ToString("R");
            var signature = MakeSignature(key: workspaceKey,
                                          method: HttpMethod.Post.Method,
                                          contentLength: encoding.GetByteCount(payload),
                                          contentType: JsonContentType,
                                          date: rfcDate,
                                          resource: "/api/logs");

            // prepare the request
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.TryAddWithoutValidation("Log-Type", logType);
            request.Headers.TryAddWithoutValidation(DateHeaderName, rfcDate);
            request.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", $"{workspaceId}:{signature}");
            if (generated != null)
            {
                request.Headers.TryAddWithoutValidation("time-generated-field", generated.Value.ToString("R"));
            }
            request.Content = new ByteArrayContent(encoding.GetBytes(payload));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);

            // make the request and get a response
            var response = await httpClient.SendAsync(request);

            // if the request was not successful, throw an exception
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new LogAnalyticsLoggerException(body);
            }
        }

        private static string MakeSignature(byte[] key,
                                            string method,
                                            int contentLength,
                                            string contentType,
                                            string date,
                                            string resource)
        {
            var stringToHash = string.Join("\n", method, contentLength, contentType, $"{DateHeaderName}:{date}", resource);
            var bytesToHash = Encoding.ASCII.GetBytes(stringToHash);
            using var sha256 = new HMACSHA256(key);
            var calculatedHash = sha256.ComputeHash(bytesToHash);
            return Convert.ToBase64String(calculatedHash);
        }
    }
}

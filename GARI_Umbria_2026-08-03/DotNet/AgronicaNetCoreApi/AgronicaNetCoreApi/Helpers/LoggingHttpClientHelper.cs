using AgronicaNetCore.Base.Models;
using Newtonsoft.Json;
using Serilog.Sinks.Http;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace AgronicaNetCoreApi.Helpers
{
    public class LoggingHttpClientHelper : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public LoggingHttpClientHelper()
        {
            _httpClient = new HttpClient();
        }

        public void Configure(IConfiguration configuration)
        {
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();
            await contentStream.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;

            string jsonBody = await new StreamReader(ms).ReadToEndAsync();
            var logEvents = JsonNode.Parse(jsonBody)?.AsArray();
            
            if (logEvents is null)
                return new HttpResponseMessage(HttpStatusCode.OK);

            foreach (var logEvent in logEvents)
            {
                var properties = logEvent?["Properties"]?.AsObject();
                if (properties is null)
                    continue;

                if (properties.ContainsKey("ElasticSearchUrl"))
                {
                    requestUri = (string)properties["ElasticSearchUrl"];
                }

                if (string.IsNullOrWhiteSpace(requestUri) || requestUri == "ElasticSearchUrl")
                    continue;

                string ambiente = string.Empty;
                string cuaa = string.Empty;
                string message = string.Empty;
                DateTime dateTime = DateTime.Now;
                string logSeverity = string.Empty;
                string stackTrace = string.Empty;

                if (properties.ContainsKey("ElasticSearchAmbiente"))
                {
                    ambiente = (string)properties["ElasticSearchAmbiente"];
                }

                if (properties.ContainsKey("CUAA"))
                {
                    cuaa = (string)properties["CUAA"];
                }

                if (properties.ContainsKey("message"))
                {
                    message = (string)properties["message"];
                }

                if (properties.ContainsKey("dateTime"))
                {
                    dateTime = (DateTime)properties["dateTime"];
                }

                if (properties.ContainsKey("logSeverity"))
                {
                    logSeverity = (string)properties["logSeverity"];
                }

                if (properties.ContainsKey("ExceptionStackTrace"))
                {
                    stackTrace = (string)properties["ExceptionStackTrace"];
                }

                var logEntry = new ElasticSearchLogEntry()
                {
                    ambiente = ambiente,
                    cuaa = cuaa,
                    componente = stackTrace,
                    opType = "LOG-APPLICATIVO",
                    timestamp = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    severity = logSeverity,
                    message = new ElasticSearchLogEntryMessage()
                    {
                        response = message,
                        payload = ""
                    },
                    task = new ElasticSearchLogEntry_Task()
                    {
                        action = "LOG-APPLICATIVO",
                        category = new List<string>() { "" },
                        cuaaEnd = cuaa,
                        cuaaInit = cuaa,
                        dateRef = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        id = ""
                    }
                };

                try
                {
                    var json = JsonConvert.SerializeObject(logEntry);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync(requestUri, content, cancellationToken);
                }
                catch (Exception ex)
                {
                    //non faccio nulla anche se la richiesta fallisce
                }
            }

            //in ogni caso restituisco status 200
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}

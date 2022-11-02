using Microsoft.Extensions.Logging;

namespace Base
{
    public class ScriptContext : IScriptContext
    {
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly string _correlationId;
        private readonly string _operationId;
        private readonly ILogger _logger;

        public ScriptContext(
            HttpClient httpClient,
            HttpRequestMessage httpRequestMessage,
            string CorrelationId,
            string OperationId,
            ILogger logger)
        {
            _httpClient = httpClient;
            _httpRequestMessage = httpRequestMessage;  
            _correlationId = CorrelationId;
            _operationId = OperationId;
            _logger = logger;
        }

        public string CorrelationId => _correlationId;

        public string OperationId => _operationId;

        public HttpRequestMessage Request => _httpRequestMessage;

        public ILogger Logger => _logger;

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(request, cancellationToken);
        }
    }
}

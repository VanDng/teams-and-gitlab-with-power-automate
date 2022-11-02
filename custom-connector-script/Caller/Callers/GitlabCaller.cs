using Base;
using Connector;
using Connector.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caller.Callers
{
    internal class GitlabCaller : BaseCaller
    {
        private readonly GitlabScript _script;

        public GitlabCaller()
        {
            _script = new GitlabScript();
        }

        public async Task<HttpResponseMessage> GetMergeRequests()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri("");

            _script.Context = 
                new ScriptContext(
                    _client,
                    request,
                    string.Empty,
                    string.Empty,
                    _logger);

            return await _script.ExecuteAsync();
        }
    }
}

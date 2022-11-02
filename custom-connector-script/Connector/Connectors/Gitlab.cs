using Base;
using Newtonsoft.Json.Linq;

namespace Connector.Connectors;

public class GitlabScript : ScriptBase, IScript
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        HttpResponseMessage response =
            await Context
                    .SendAsync(Context.Request, CancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);

        if (response.IsSuccessStatusCode)
        {
            IDictionary<string, string?> header =
                response.Headers
                        .Select
                        (
                            s => new KeyValuePair<string, string?>(s.Key, s.Value?.FirstOrDefault())
                        )
                        .ToDictionary(s => s.Key, s => s.Value);

            string body =
                await response.Content
                                .ReadAsStringAsync()
                                .ConfigureAwait(continueOnCapturedContext: false);

            var newResult = new JObject
            {
                ["header"] = JToken.FromObject(header),
                ["body"] = JToken.Parse(body)
            };

            response.Content = CreateJsonContent(newResult.ToString());
        }

        return response;
    }
}


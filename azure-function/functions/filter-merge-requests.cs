using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using azure_function.db;

namespace azure_function.functions
{
    public static class filter_merge_request
    {
        [FunctionName("filter_merge_request")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            JObject requestObject = (JObject)JsonConvert.DeserializeObject(requestBody);

            string projectId = requestObject["project_id"].Value<string>();
            JArray mergeRequestArray = requestObject["body"].Value<JArray>();

            JArray newMergeRequestArray = Filter(projectId, mergeRequestArray);

            return new OkObjectResult(newMergeRequestArray);
        }

        private static JArray Filter(string projectId, JArray originalArray)
        {
            var newArray = new JArray();

            IReadOnlyCollection<string> iids =
                originalArray.Cast<JObject>()
                .Select(s => s["iid"].Value<string>()).ToList();

            IReadOnlyCollection<MergeRequestTrackingItem> trackingItems =
                AzureTableClient.GetMergeRequestTrackingItems(projectId, iids);

            foreach (JObject mergeRequest in originalArray.Cast<JObject>())
            {
                string iid = mergeRequest["iid"].Value<int>().ToString();
                //string updated_at = mergeRequest["updated_at"].Value<string>();

                MergeRequestTrackingItem trackingItem = trackingItems.FirstOrDefault(item => item.RowKey == iid);

                if (trackingItem is null)
                {
                    mergeRequest.Add($"{nameof(MergeRequestTrackingItem.adaptive_card_team_message_id)}", null);

                    newArray.Add(mergeRequest);

                    continue;
                }

                //if (trackingItem.updated_at != updated_at)
                //{
                mergeRequest.Add($"{nameof(MergeRequestTrackingItem.adaptive_card_team_message_id)}",
                        trackingItem.adaptive_card_team_message_id is null
                        ? null
                        : JToken.FromObject(trackingItem.adaptive_card_team_message_id));

                newArray.Add(mergeRequest);
                //}
            }

            return newArray;
        }
    }
}

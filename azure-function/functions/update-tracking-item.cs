using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Azure.Data.Tables;
using Azure;
using azure_function.db;

namespace azure_function.functions
{
    public static class update_tracking_item
    {
        [FunctionName("update_tracking_item")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject updateTrackingItem = (JObject)JsonConvert.DeserializeObject(requestBody);

            string projectId = updateTrackingItem["project_id"].Value<string>();
            string iid = updateTrackingItem["iid"].Value<string>();
            string updated_at = updateTrackingItem["updated_at"].Value<string>();
            string adaptive_card_team_message_id = updateTrackingItem["adaptive_card_team_message_id"].Value<string>();

            MergeRequestTrackingItem trackingItem = AzureTableClient.GetMergeRequestTrackingItems(projectId, new string[]
            {
                iid
            }).FirstOrDefault();

            if (trackingItem is null)
            {
                AzureTableClient.AddMergeRequestTrackingItem(new MergeRequestTrackingItem()
                {
                    PartitionKey = projectId,
                    RowKey = iid,
                    updated_at = updated_at,
                    adaptive_card_team_message_id = adaptive_card_team_message_id
                });

                return new OkResult();
            }

            trackingItem.updated_at = updated_at;
            trackingItem.adaptive_card_team_message_id = adaptive_card_team_message_id;
            AzureTableClient.UpdateMergeRequestTrackingItem(trackingItem);

            return new OkResult();
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using azure_function.db;
using Newtonsoft.Json.Linq;

namespace azure_function.functions
{
    public static class release_flow_lock
    {
        [FunctionName("release_flow_lock")]
        [Singleton(Mode = SingletonMode.Function)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            JObject requestObject = (JObject)JsonConvert.DeserializeObject(requestBody);

            string flowId = requestObject["flow_id"].Value<string>();

            FlowTrackingItem flowTrackingItem =
                AzureTableClient.GetFlowTrackingItem(flowId);

            flowTrackingItem.last_update = DateTime.UtcNow;
            flowTrackingItem.status = FlowStatus.Idle;

            AzureTableClient.UpdateFlowTrackingItem(flowTrackingItem);

            return new OkResult();
        }
    }
}

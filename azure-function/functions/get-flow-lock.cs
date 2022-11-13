using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using azure_function.db;
using System.IO.Pipes;

namespace azure_function.functions
{
    public static class get_flow_lock
    {
        [FunctionName("get_flow_lock")]
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

            if (flowTrackingItem is null)
            {
                AzureTableClient.AddFlowTrackingItem(new FlowTrackingItem()
                {
                    PartitionKey = flowId,
                    RowKey = flowId,
                    last_update = DateTime.UtcNow,
                    status = FlowStatus.Running
                });

                return MakeFlowStatus(FlowStatus.Idle);
            }

            if (flowTrackingItem.status == FlowStatus.Running)
            {
                // It has been locking for 5 minutes.
                // Break the lock.
                if (DateTime.UtcNow.Subtract(flowTrackingItem.last_update).TotalMinutes > 5)
                {
                    flowTrackingItem.last_update = DateTime.UtcNow;
                    AzureTableClient.UpdateFlowTrackingItem(flowTrackingItem);

                    return MakeFlowStatus(FlowStatus.Idle);
                }

                return MakeFlowStatus(FlowStatus.Running);
            }

            if (flowTrackingItem.status == FlowStatus.Idle)
            {
                flowTrackingItem.last_update = DateTime.UtcNow;
                flowTrackingItem.status = FlowStatus.Running;
                AzureTableClient.UpdateFlowTrackingItem(flowTrackingItem);

                return MakeFlowStatus(FlowStatus.Idle);
            }

            return new OkObjectResult("Un-handled cases. Should never happen.");
        }

        private static OkObjectResult MakeFlowStatus(FlowStatus status)
        {
            return new OkObjectResult(JToken.FromObject(new
            {
                status = status.ToString()
            }));
        }
    }
}

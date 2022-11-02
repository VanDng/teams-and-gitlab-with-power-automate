using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace azure_function
{
    public static class AzureTableClient
    {
        private static TableClient _tableClient;

        private static void Initialize()
        {
            var tableServiceClient =
                new TableServiceClient(
                    Environment.GetEnvironmentVariable("AZURE_TABLE_CONNECTION_STRING", EnvironmentVariableTarget.Process));
                    
            _tableClient =
                tableServiceClient.GetTableClient("mergerequesttracking");

            _tableClient.CreateIfNotExists();
        }

        public static TableClient GetTableClient()
        {
            if (_tableClient is null)
            {
                Initialize();
            }

            return _tableClient;
        }

        public static IReadOnlyCollection<TrackingItem> GetTrackingItems(string projectId, IReadOnlyCollection<string> iids)
        {
            IReadOnlyCollection<string> iidConditions =
             iids.Select(iid => $"RowKey eq '{iid}'").ToList();

            string queryCondition = string.Join(" or ", iidConditions);

            string query =
                $"PartitionKey eq '{projectId}' and ({queryCondition})";

            IReadOnlyCollection<TrackingItem> trackingItems =
                   GetTableClient()
                    .Query<TrackingItem>(query).ToList();

            return trackingItems;
        }
    }
}

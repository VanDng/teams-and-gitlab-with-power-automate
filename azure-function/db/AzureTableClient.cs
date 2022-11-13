using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace azure_function.db
{
    public static class AzureTableClient
    {
        private static TableClient _mergeRequestTrackingTable;
        private static TableClient _flowTable;

        private static void Initialize()
        {
            var tableServiceClient =
                new TableServiceClient(
                    Environment.GetEnvironmentVariable("AZURE_TABLE_CONNECTION_STRING", EnvironmentVariableTarget.Process));

            _mergeRequestTrackingTable =
                tableServiceClient.GetTableClient("mergerequesttracking");
            _mergeRequestTrackingTable.CreateIfNotExists();

            _flowTable =
               tableServiceClient.GetTableClient("flowtracking");
            _flowTable.CreateIfNotExists();
        }

        #region MergeRequestTracking

        private static TableClient GetMergeRequestTrackingTable()
        {
            if (_mergeRequestTrackingTable is null)
            {
                Initialize();
            }

            return _mergeRequestTrackingTable;
        }

        public static void AddMergeRequestTrackingItem(MergeRequestTrackingItem item)
        {
            GetMergeRequestTrackingTable().AddEntity(item);
        }

        public static void UpdateMergeRequestTrackingItem(MergeRequestTrackingItem item)
        {
            GetMergeRequestTrackingTable().UpdateEntity(item, ETag.All, TableUpdateMode.Replace);
        }

        public static IReadOnlyCollection<MergeRequestTrackingItem> GetMergeRequestTrackingItems(string projectId, IReadOnlyCollection<string> iids)
        {
            IReadOnlyCollection<string> iidConditions =
             iids.Select(iid => $"RowKey eq '{iid}'").ToList();

            string queryCondition = string.Join(" or ", iidConditions);

            string query =
                $"PartitionKey eq '{projectId}' and ({queryCondition})";

            IReadOnlyCollection<MergeRequestTrackingItem> trackingItems =
                   GetMergeRequestTrackingTable()
                    .Query<MergeRequestTrackingItem>(query).ToList();

            return trackingItems;
        }

        #endregion MergeRequestTracking

        #region FlowTracking

        private static TableClient GetFlowTrackingTable()
        {
            if (_flowTable is null)
            {
                Initialize();
            }

            return _flowTable;
        }

        public static FlowTrackingItem GetFlowTrackingItem(string flowId)
        {
            string query =
                $"PartitionKey eq '{flowId}' and RowKey eq '{flowId}'";

            IReadOnlyCollection<FlowTrackingItem> trackingItems =
                   GetFlowTrackingTable().Query<FlowTrackingItem>(query).ToList();

            if (trackingItems.Count > 0)
            {
                return trackingItems.First();
            }

            return null;
        }

        public static void AddFlowTrackingItem(FlowTrackingItem item)
        {
            GetFlowTrackingTable().AddEntity(item);
        }

        public static void UpdateFlowTrackingItem(FlowTrackingItem item)
        {
            GetFlowTrackingTable().UpdateEntity(item, ETag.All, TableUpdateMode.Replace);
        }

        #endregion FlowTracking
    }
}

using Azure;
using Azure.Data.Tables;
using System;

namespace azure_function.db
{
    public class FlowTrackingItem : ITableEntity
    {
        public string PartitionKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string RowKey { get; set; }

        public DateTime last_update { get; set; }
        public FlowStatus status { get; set; }
    }
}

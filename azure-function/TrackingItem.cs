using Azure;
using Azure.Data.Tables;
using System;

namespace azure_function
{
    public class TrackingItem : ITableEntity
    {
        public string PartitionKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string RowKey { get; set; }

        public string updated_at { get; set; }
        public string adaptive_card_team_message_id { get; set; }
    }
}

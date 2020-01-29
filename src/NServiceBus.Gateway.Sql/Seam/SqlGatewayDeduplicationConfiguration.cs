using NServiceBus.ObjectBuilder;
using NServiceBus.Settings;
using System;

namespace NServiceBus.Gateway.Sql
{
    /// <summary>
    /// Configuration class for the SQL gateway deduplication storage
    /// </summary>
    public class SqlGatewayDeduplicationConfiguration : GatewayDeduplicationConfiguration
    {
        string connectionString;
        string schema = "dbo";
        string tableName = "GatewayDeduplication";

        /// <summary>
        /// Configures the connection string to use for the deduplication storage.
        /// </summary>
        public string ConnectionString
        {
            get => connectionString;
            set
            {
                Guard.AgainstNullAndEmpty(nameof(value), value);
                connectionString = value;
            }
        }


        /// <summary>
        /// Configures the schema to use for the deduplication storage. Defaults to `dbo`.
        /// </summary>
        public string Schema
        {
            get => schema;
            set
            {
                Guard.AgainstNullAndEmpty(nameof(value), value);
                schema = value;
            }
        }

        /// <summary>
        /// Configures the table name to use for the deduplication storage. Defaults to `GatewayDeduplication`.
        /// </summary>
        public string TableName
        {
            get => tableName;
            set
            {
                Guard.AgainstNullAndEmpty(nameof(value), value);
                tableName = value;
            }
        }

        /// <inheritdoc />
        public override IGatewayDeduplicationStorage CreateStorage(IBuilder builder)
        {
            if(string.IsNullOrEmpty(connectionString))
            {
                // TODO: Better exception
                throw new Exception("No connection string");
            }

            var sqlSettings = new SqlSettings(connectionString, schema, tableName);

            return new SqlGatewayDeduplicationStorage(sqlSettings);
        }
    }
}

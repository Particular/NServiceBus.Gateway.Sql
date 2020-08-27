namespace NServiceBus
{
    using Gateway;
    using Gateway.Sql;
    using System;
    using System.Data.Common;

    /// <summary>
    /// Configuration class for the SQL gateway deduplication storage
    /// </summary>
    public class SqlGatewayDeduplicationConfiguration : GatewayDeduplicationConfiguration
    {
        internal Func<IServiceProvider, DbConnection> connectionBuilder;
        string schema = "dbo";
        string tableName = "GatewayDeduplication";

        /// <summary>
        /// TODO
        /// </summary>
        public void ConnectionBuilder(Func<DbConnection> connectionBuilder)
        {
            this.connectionBuilder = _ => connectionBuilder();
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void ConnectionBuilder(Func<IServiceProvider, DbConnection> connectionBuilder)
        {
            this.connectionBuilder = connectionBuilder;
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
        public override IGatewayDeduplicationStorage CreateStorage(IServiceProvider builder)
        {
            if(connectionBuilder == null)
            {
                // TODO: Better exception
                throw new Exception("No connection builder");
            }

            var sqlSettings = new SqlSettings(connectionBuilder, schema, tableName);

            return new SqlGatewayDeduplicationStorage(builder, sqlSettings);
        }
    }
}

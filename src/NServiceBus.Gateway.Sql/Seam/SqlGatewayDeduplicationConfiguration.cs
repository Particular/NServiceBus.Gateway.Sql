namespace NServiceBus
{
    using System;
    using System.Data.Common;
    using Gateway;
    using Gateway.Sql;

    /// <summary>
    /// Configuration class for the SQL gateway deduplication storage
    /// </summary>
    public class SqlGatewayDeduplicationConfiguration : GatewayDeduplicationConfiguration
    {
        internal Func<IServiceProvider, DbConnection> connectionBuilder;

        /// <summary>
        /// Configures a custom connection builder.
        /// </summary>
        public void ConnectionBuilder(Func<DbConnection> connectionBuilder)
        {
            this.connectionBuilder = _ => connectionBuilder();
        }

        /// <summary>
        /// Configures a custom connection builder.
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
            get;
            set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                field = value;
            }
        } = "dbo";

        /// <summary>
        /// Configures the table name to use for the deduplication storage. Defaults to `GatewayDeduplication`.
        /// </summary>
        public string TableName
        {
            get;
            set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                field = value;
            }
        } = "GatewayDeduplication";

        /// <inheritdoc />
        public override IGatewayDeduplicationStorage CreateStorage(IServiceProvider builder)
        {
            if (connectionBuilder == null)
            {
                // TODO: Better exception
                throw new Exception("No connection builder");
            }

            var sqlSettings = new SqlSettings(connectionBuilder, Schema, TableName);

            return new SqlGatewayDeduplicationStorage(builder, sqlSettings);
        }
    }
}
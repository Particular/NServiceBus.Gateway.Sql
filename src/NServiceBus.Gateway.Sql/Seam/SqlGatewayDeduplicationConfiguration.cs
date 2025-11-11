namespace NServiceBus
{
    using System;
    using System.Data.Common;
    using Features;
    using Gateway;
    using Gateway.Sql;
    using Microsoft.Extensions.DependencyInjection;
    using Settings;

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
        protected override void EnableFeature(SettingsHolder settings) => settings.EnableFeature<SqlGatewayDeduplication>();

        class SqlGatewayDeduplication : Feature
        {
            protected override void Setup(FeatureConfigurationContext context)
            {
                var configuration = context.Settings.Get<SqlGatewayDeduplicationConfiguration>();

                if (configuration.connectionBuilder == null)
                {
                    throw new Exception($"No connection builder was configured. Use {nameof(ConnectionBuilder)}(myConnectionBuilder) to configure one.");
                }

                var sqlSettings = new SqlSettings(configuration.connectionBuilder, configuration.Schema, configuration.TableName);

                context.Services.AddSingleton<IGatewayDeduplicationStorage>(sp => new SqlGatewayDeduplicationStorage(sp, sqlSettings));

                context.AddInstaller<SqlGatewayDeduplicationInstaller>();
            }
        }
    }
}
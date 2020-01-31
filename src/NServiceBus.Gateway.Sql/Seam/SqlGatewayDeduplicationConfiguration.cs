using NServiceBus.ObjectBuilder;
using NServiceBus.Settings;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    /// <summary>
    /// Configuration class for the SQL gateway deduplication storage
    /// </summary>
    public class SqlGatewayDeduplicationConfiguration : GatewayDeduplicationConfiguration
    {
        Func<IBuilder, DbConnection> connectionBuilder;
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
        public void ConnectionBuilder(Func<IBuilder, DbConnection> connectionBuilder)
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
        public override IGatewayDeduplicationStorage CreateStorage(IBuilder builder)
        {
            if(connectionBuilder == null)
            {
                // TODO: Better exception
                throw new Exception("No connection builder");
            }

            var sqlSettings = new SqlSettings(connectionBuilder, schema, tableName);

            return new SqlGatewayDeduplicationStorage(builder, sqlSettings);
        }

        /// <inheritdoc />
        public override void Setup(ReadOnlySettings settings)
        {
            bool installersEnabled = settings.Get<bool>("Installers.Enable");

            if(installersEnabled)
            {
                using (var connection = connectionBuilder(null))
                {
                    CreateDeduplicationTable(connection);
                }
            }
        }

        /// <summary>
        /// Given a connection, create a duplication table using the Schema and TableName defined by this class
        /// </summary>
        public void CreateDeduplicationTable(IDbConnection connection)
        {
            var sql = $@"
declare @ObjectId int = object_id('[{Schema}].[{TableName}]')

if not exists (
	select * from sys.objects
	where
		object_id = @ObjectId
		and type = 'U'
)
begin
	
	create table [{Schema}].[{TableName}] (
		Id nvarchar(255) not null primary key clustered,
		TimeReceived datetime null
	)

end

if not exists (
	select *
	from sys.indexes
	where
		name = 'Index_TimeReceived'
		and object_id = @ObjectId
)
begin

	create index Index_TimeReceived
	on [{Schema}].[{TableName}] (TimeReceived asc)

end";
            connection.Open();

            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }

        }
    }
}

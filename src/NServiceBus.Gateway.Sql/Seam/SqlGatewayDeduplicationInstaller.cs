﻿using NServiceBus.Installation;
using NServiceBus.ObjectBuilder;
using NServiceBus.Settings;
using System.Data;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlGatewayDeduplicationInstaller : INeedToInstallSomething
    {
        readonly ReadOnlySettings settings;
        readonly IBuilder builder;

        public SqlGatewayDeduplicationInstaller(ReadOnlySettings settings, IBuilder builder)
        {
            this.settings = settings;
            this.builder = builder;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities",
    Justification = "String formatting only for schema and table name")]
        public async Task Install(string identity)
        {
            var config = settings.GetOrDefault<GatewayDeduplicationConfiguration>() as SqlGatewayDeduplicationConfiguration;

            if(config == null)
            {
                return;
            }

            var fullName = $"[{config.Schema}].[{config.TableName}]";

            var sql = $@"
if not exists (
	select * from sys.objects
	where
		object_id = object_id('{fullName}')
		and type = 'U'
)
begin
	
	create table {fullName} (
		Id nvarchar(255) not null primary key clustered,
		TimeReceived datetime null
	)

end

if not exists (
	select *
	from sys.indexes
	where
		name = 'Index_TimeReceived'
		and object_id = object_id('{fullName}')
)
begin

	create index Index_TimeReceived
	on {fullName} (TimeReceived asc)

end";
            using (var connection = config.connectionBuilder(builder))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    transaction.Commit();
                }
            }    
        }
    }
}
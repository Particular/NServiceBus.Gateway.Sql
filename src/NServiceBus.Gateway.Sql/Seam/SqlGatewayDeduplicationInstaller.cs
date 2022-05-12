namespace NServiceBus.Gateway.Sql
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Installation;
    using Settings;

    class SqlGatewayDeduplicationInstaller : INeedToInstallSomething
    {
        readonly IReadOnlySettings settings;
        readonly IServiceProvider builder;

        public SqlGatewayDeduplicationInstaller(IReadOnlySettings settings, IServiceProvider builder)
        {
            this.settings = settings;
            this.builder = builder;
        }

        public async Task Install(string identity, CancellationToken cancellationToken = default)
        {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0078 // Use pattern matching (may change code meaning)
#pragma warning disable IDE0083 // Use pattern matching
            if (!(settings.GetOrDefault<GatewayDeduplicationConfiguration>() is SqlGatewayDeduplicationConfiguration config))
            {
                return;
            }
#pragma warning restore IDE0083 // Use pattern matching
#pragma warning restore IDE0078 // Use pattern matching (may change code meaning)
#pragma warning restore IDE0079 // Remove unnecessary suppression

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
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
            }
        }
    }
}

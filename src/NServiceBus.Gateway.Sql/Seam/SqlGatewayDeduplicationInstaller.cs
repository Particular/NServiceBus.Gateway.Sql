namespace NServiceBus.Gateway.Sql;

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Installation;
using Settings;

class SqlGatewayDeduplicationInstaller(IReadOnlySettings settings, IServiceProvider serviceProvider) : INeedToInstallSomething
{
    public async Task Install(string identity, CancellationToken cancellationToken = default)
    {
        var config = settings.Get<SqlGatewayDeduplicationConfiguration>();
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

        var connection = config.connectionBuilder(serviceProvider);
        await using var _ = connection.ConfigureAwait(false);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken).ConfigureAwait(false);
        await using var __ = transaction.ConfigureAwait(false);
        var cmd = connection.CreateCommand();
        await using var ___ = cmd.ConfigureAwait(false);
        cmd.Transaction = transaction;
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
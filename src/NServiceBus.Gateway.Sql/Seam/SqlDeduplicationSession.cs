namespace NServiceBus;

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Gateway;
using Gateway.Sql;

class SqlDeduplicationSession(
    string messageId,
    SqlSettings settings,
    bool isDuplicate,
    DbConnection connection,
    DbTransaction transaction)
    : IDeduplicationSession
{
    public bool IsDuplicate { get; } = isDuplicate;

    public async Task MarkAsDispatched(CancellationToken cancellationToken = default)
    {
        var cmd = connection.CreateCommand();
        await using (cmd.ConfigureAwait(false))
        {
            cmd.Transaction = transaction;
            cmd.CommandText = settings.MarkDispatchedSql;
            cmd.AddParameter("Id", messageId);

            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose() => Dispose(true);
}
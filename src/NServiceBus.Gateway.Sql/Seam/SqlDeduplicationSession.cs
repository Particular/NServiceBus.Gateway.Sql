namespace NServiceBus
{
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Gateway;
    using Gateway.Sql;

    class SqlDeduplicationSession : IDeduplicationSession
    {
        readonly string messageId;
        readonly SqlSettings settings;
        readonly DbConnection connection;
        readonly DbTransaction transaction;

        public SqlDeduplicationSession(string messageId, SqlSettings settings, bool isDuplicate, DbConnection connection, DbTransaction transaction)
        {
            this.messageId = messageId;
            this.settings = settings;
            this.connection = connection;
            this.transaction = transaction;

            IsDuplicate = isDuplicate;
        }

        public bool IsDuplicate { get; }

        public async Task MarkAsDispatched(CancellationToken cancellationToken = default)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = settings.MarkDispatchedSql;
                cmd.AddParameter("Id", messageId);

                await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            transaction.Commit();
        }

        #region IDisposable Support
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

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

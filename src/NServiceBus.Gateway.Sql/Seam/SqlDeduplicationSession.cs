using NServiceBus.Gateway;
using NServiceBus.Gateway.Sql;
using System.Data.Common;
using System.Threading.Tasks;

namespace NServiceBus
{
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "String formatting only for schema and table name")]
        public async Task MarkAsDispatched()
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = settings.MarkDispatchedSql;
                cmd.AddParameter("Id", messageId);

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            transaction.Commit();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

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

using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlDeduplicationSession : IDeduplicationSession
    {
        readonly string messageId;
        readonly SqlSettings settings;

        DbConnection connection;
        DbTransaction transaction; 

        public SqlDeduplicationSession(string messageId, SqlSettings settings)
        {
            this.messageId = messageId;
            this.settings = settings;
        }

        public bool IsDuplicate
        {
            get
            {
                OpenConnection();

                using (var cmd = settings.GetIsDuplicateCommand(connection, transaction, messageId))
                {
                    var result = cmd.ExecuteScalar();
                    return result != null;
                }
            }
        }

        public async Task MarkAsDispatched()
        {
            OpenConnection();

            using (var cmd = settings.GetMarkAsDispatchedCommand(connection, transaction, messageId))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
        }

        void OpenConnection()
        {
            if(connection == null)
            {
                connection = settings.CreateConnection();
                connection.Open();
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            }
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

                // TODO: set large fields to null.

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

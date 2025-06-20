namespace NServiceBus.Gateway.Sql
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensibility;

    class SqlGatewayDeduplicationStorage : IGatewayDeduplicationStorage
    {
        readonly SqlSettings settings;
        readonly IServiceProvider builder;

        public SqlGatewayDeduplicationStorage(IServiceProvider builder, SqlSettings settings)
        {
            this.builder = builder;
            this.settings = settings;
        }

        public bool SupportsDistributedTransactions => true;

        public async Task<IDeduplicationSession> CheckForDuplicate(string messageId, ContextBag context, CancellationToken cancellationToken = default)
        {
            var connection = settings.ConnectionBuilder(builder);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken).ConfigureAwait(false);

            var distributedTransaction = System.Transactions.Transaction.Current;
            if (distributedTransaction != null)
            {
                connection.EnlistTransaction(distributedTransaction);
            }

            var isDuplicate = await IsDuplicate(connection, transaction, messageId, cancellationToken).ConfigureAwait(false);

            return new SqlDeduplicationSession(messageId, settings, isDuplicate, connection, transaction);
        }

        public async Task<bool> IsDuplicate(DbConnection connection, DbTransaction transaction, string messageId, CancellationToken cancellationToken = default)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = settings.IsDuplicateSql;
                cmd.AddParameter("Id", messageId);

                var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                return result != null;
            }
        }
    }
}
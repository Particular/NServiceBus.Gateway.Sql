﻿namespace NServiceBus.Gateway.Sql
{
    using Extensibility;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

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

        public async Task<IDeduplicationSession> CheckForDuplicate(string messageId, ContextBag context)
        {
            var connection = settings.ConnectionBuilder(builder);
            await connection.OpenAsync().ConfigureAwait(false);

            // DbTransaction gets Async overloads starting in netstandard2.1/netcoreapp3.0
            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            var distributedTransaction = System.Transactions.Transaction.Current;
            if (distributedTransaction != null)
            {
                connection.EnlistTransaction(distributedTransaction);
            }

            var isDuplicate = await IsDuplicate(connection, transaction, messageId).ConfigureAwait(false);

            return new SqlDeduplicationSession(messageId, settings, isDuplicate, connection, transaction);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "String formatting only for schema and table name")]
        public async Task<bool> IsDuplicate(DbConnection connection, DbTransaction transaction, string messageId)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = settings.IsDuplicateSql;
                cmd.AddParameter("Id", messageId);

                var result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                return result != null;
            }
        }
    }
}

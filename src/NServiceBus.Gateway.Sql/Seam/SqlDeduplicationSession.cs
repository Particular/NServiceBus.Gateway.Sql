using NServiceBus.ObjectBuilder;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlDeduplicationSession : IDeduplicationSession
    {
        readonly IBuilder builder;
        readonly string messageId;
        readonly SqlSettings settings;

        DbConnection connection;
        DbTransaction transaction; 

        public SqlDeduplicationSession(IBuilder builder, string messageId, SqlSettings settings)
        {
            this.builder = builder;
            this.messageId = messageId;
            this.settings = settings;
        }

        public bool IsDuplicate
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]

            get
            {
                OpenConnection();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = settings.IsDuplicateSql;
                    AddParameter(cmd, "Id", messageId);

                    var result = cmd.ExecuteScalar();
                    return result != null;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public async Task MarkAsDispatched()
        {
            OpenConnection();

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = settings.MarkDispatchedSql;
                AddParameter(cmd, "Id", messageId);

                await cmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
        }

        void OpenConnection()
        {
            if(connection == null)
            {
                connection = settings.ConnectionBuilder(builder);
                connection.Open();
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            }
        }

        static void AddParameter(DbCommand cmd, string name, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
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

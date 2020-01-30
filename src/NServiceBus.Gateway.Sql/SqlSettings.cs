using NServiceBus.ObjectBuilder;
using System;
using System.Data.Common;
using System.Reflection;

namespace NServiceBus.Gateway.Sql
{
    class SqlSettings
    {
        readonly Func<IBuilder, DbConnection> connectionBuilder;
        readonly string isDuplicateSql;
        readonly string markDispatchedSql;

        public SqlSettings(Func<IBuilder, DbConnection> connectionBuilder, string schema, string tableName)
        {
            this.connectionBuilder = connectionBuilder;

            isDuplicateSql = $"select top 1 TimeReceived from  [{schema}].[{tableName}] where Id = @Id";
            markDispatchedSql = $"insert into [{schema}].[{tableName}] (Id, TimeReceived) values (@Id, GETUTCDATE())";
        }

        public DbConnection CreateConnection()
        {
            return connectionBuilder(null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public DbCommand GetIsDuplicateCommand(DbConnection connection, DbTransaction transaction, string messageId)
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = isDuplicateSql;
            AddParameter(cmd, "Id", messageId);
            return cmd;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public DbCommand GetMarkAsDispatchedCommand(DbConnection connection, DbTransaction transaction, string messageId)
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = markDispatchedSql;
            AddParameter(cmd, "Id", messageId);
            return cmd;
        }

        static void AddParameter(DbCommand cmd, string name, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }
    }
}

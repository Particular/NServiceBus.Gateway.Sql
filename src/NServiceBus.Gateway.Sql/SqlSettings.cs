using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlSettings
    {
        readonly string connectionString;
        readonly string isDuplicateSql;
        readonly string markDispatchedSql;
        readonly DbProviderFactory factory;

        public SqlSettings(string connectionString, string schema, string tableName)
        {
            this.connectionString = connectionString;

            isDuplicateSql = $"select top 1 TimeReceived from  [{schema}].[{tableName}] where Id = @Id";
            markDispatchedSql = $"insert into [{schema}].[{tableName}] (Id, TimeReceived) values (@Id, GETUTCDATE())";

            factory = GetFactory("System.Data.SqlClient.SqlClientFactory, System.Data") ??
                GetFactory("Microsoft.Data.SqlClientFactory, Microsoft.Data.SqlClient");

            if(factory == null)
            {
                // TODO: Better exception
                throw new Exception("No factory");
            }
        }

        public DbConnection CreateConnection()
        {
            var conn = factory.CreateConnection();
            conn.ConnectionString = connectionString;
            return conn;
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

        static DbProviderFactory GetFactory(string assemblyQualifiedName)
        {
            var type = Type.GetType("System.Data.SqlClient.SqlClientFactory, System.Data", throwOnError: false, ignoreCase: false);

            var instanceFieldInfo = type?.GetField("Instance", BindingFlags.Public | BindingFlags.Static);

            return instanceFieldInfo?.GetValue(null) as DbProviderFactory;
        }
    }
}

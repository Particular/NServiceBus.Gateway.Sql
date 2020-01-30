using NServiceBus.ObjectBuilder;
using System;
using System.Data.Common;
using System.Reflection;

namespace NServiceBus.Gateway.Sql
{
    class SqlSettings
    {
        public Func<IBuilder, DbConnection> ConnectionBuilder { get; private set; }
        public string IsDuplicateSql { get; private set; }
        public string MarkDispatchedSql { get; private set; }

        public SqlSettings(Func<IBuilder, DbConnection> connectionBuilder, string schema, string tableName)
        {
            this.ConnectionBuilder = connectionBuilder;

            IsDuplicateSql = $"select top 1 TimeReceived from  [{schema}].[{tableName}] where Id = @Id";
            MarkDispatchedSql = $"insert into [{schema}].[{tableName}] (Id, TimeReceived) values (@Id, GETUTCDATE())";
        }
    }
}

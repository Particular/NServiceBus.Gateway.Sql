namespace NServiceBus.Gateway.Sql;

using System;
using System.Data.Common;

class SqlSettings(Func<IServiceProvider, DbConnection> connectionBuilder, string schema, string tableName)
{
    public Func<IServiceProvider, DbConnection> ConnectionBuilder { get; private set; } = connectionBuilder;
    public string IsDuplicateSql { get; private set; } = $"select top 1 TimeReceived from  [{schema}].[{tableName}] where Id = @Id";
    public string MarkDispatchedSql { get; private set; } = $"insert into [{schema}].[{tableName}] (Id, TimeReceived) values (@Id, GETUTCDATE())";
}
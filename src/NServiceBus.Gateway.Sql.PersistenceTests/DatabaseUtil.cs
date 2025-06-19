using System;
using Microsoft.Data.SqlClient;

public static class DatabaseUtil
{
    const string ConnectionString = @"Server=localhost\sqlexpress;Database=nservicebus;Trusted_Connection=True;";

    public static SqlConnection Build() => new(GetConnectionString());

    public static void DropDbIfCollationIncorrect()
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
        var databaseName = connectionStringBuilder.InitialCatalog;

        connectionStringBuilder.InitialCatalog = "master";

        using var connection = new SqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM sys.databases WHERE name = '{databaseName}' AND COALESCE(collation_name, '') <> 'SQL_Latin1_General_CP1_CS_AS'";

        using var reader = command.ExecuteReader();
        if (reader.HasRows) // The database collation is wrong, drop so that it will be recreated
        {
            DropDatabase(databaseName);
        }
    }

    public static void CreateDbIfNotExists()
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
        var databaseName = connectionStringBuilder.InitialCatalog;

        connectionStringBuilder.InitialCatalog = "master";

        using var connection = new SqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"select * from master.dbo.sysdatabases where name='{databaseName}'";

        using (var reader = command.ExecuteReader())
        {
            if (reader.HasRows) // exists
            {
                return;
            }
        }

        command.CommandText = $"CREATE DATABASE {databaseName} COLLATE SQL_Latin1_General_CP1_CS_AS";
        command.ExecuteNonQuery();
    }

    public static void CreateDeduplicationTableIfNotExists()
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());

        using var connection = new SqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        using var command = connection.CreateCommand();
        const string fullName = "GatewayDeduplication";
        command.CommandText = $"""

                               if not exists (
                               	select * from sys.objects
                               	where
                               		object_id = object_id('{fullName}')
                               		and type = 'U'
                               )
                               begin

                               	create table {fullName} (
                               		Id nvarchar(255) not null primary key clustered,
                               		TimeReceived datetime null
                               	)

                               end

                               if not exists (
                               	select *
                               	from sys.indexes
                               	where
                               		name = 'Index_TimeReceived'
                               		and object_id = object_id('{fullName}')
                               )
                               begin

                               	create index Index_TimeReceived
                               	on {fullName} (TimeReceived asc)

                               end
                               """;
        command.ExecuteNonQuery();
    }

    public static string GetConnectionString()
    {
        var connection = Environment.GetEnvironmentVariable("SQLServerConnectionString");

        return string.IsNullOrWhiteSpace(connection) ? ConnectionString : connection;
    }

    static void DropDatabase(string databaseName)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(GetConnectionString())
        {
            InitialCatalog = "master"
        };

        using var connection = new SqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        using var dropCommand = connection.CreateCommand();
        dropCommand.CommandText = $"use master; if exists(select * from sysdatabases where name = '{databaseName}') begin alter database {databaseName} set SINGLE_USER with rollback immediate; drop database {databaseName}; end; ";
        dropCommand.ExecuteNonQuery();
    }
}
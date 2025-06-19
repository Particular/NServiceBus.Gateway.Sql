
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Gateway;

partial class GatewayPersistenceTestsConfiguration : IGatewayPersistenceTestsConfiguration
{
    const string ConnectionString = @"Server=localhost\sqlexpress;Database=nservicebus;Trusted_Connection=True;";

    public IGatewayDeduplicationStorage CreateStorage()
    {
        var connectionString = GetConnectionString();

        var config = new SqlGatewayDeduplicationConfiguration();

        config.ConnectionBuilder(builder => new SqlConnection(connectionString));

        var storage = config.CreateStorage(new ServiceCollection().BuildServiceProvider());

        return storage;
    }

    public static string GetConnectionString()
    {
        var connection = Environment.GetEnvironmentVariable("SQLServerConnectionString");

        return string.IsNullOrWhiteSpace(connection) ? ConnectionString : connection;
    }
}
﻿namespace NServiceBus.Gateway.AcceptanceTests
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using NServiceBus.AcceptanceTesting.Support;

    public partial class GatewayTestSuiteConstraints
    {
        public Task<GatewayDeduplicationConfiguration> ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            var connectionString = DatabaseUtil.GetConnectionString();

            var config = new SqlGatewayDeduplicationConfiguration
            {
                TableName = Regex.Replace(endpointName, "[^A-Za-z0-9]+", "") + "_GatewayDeduplication"
            };
            config.ConnectionBuilder(builder => new SqlConnection(connectionString));

            return Task.FromResult<GatewayDeduplicationConfiguration>(config);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

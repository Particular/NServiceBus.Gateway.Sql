namespace NServiceBus.Gateway.AcceptanceTests
{
    using NServiceBus.AcceptanceTesting.Support;
    using NServiceBus.Gateway.Sql;
#if MicrosoftDataClient
    using Microsoft.Data.SqlClient;
#elif SystemDataClient
    using System.Data.SqlClient;
#endif
    using System.Threading.Tasks;

    public partial class GatewayTestSuiteConstraints
    {
        public Task<GatewayDeduplicationConfiguration> ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            var connectionString = DatabaseUtil.GetConnectionString();

            var config = new SqlGatewayDeduplicationConfiguration();
            config.ConnectionBuilder(builder => new SqlConnection(connectionString));

            return Task.FromResult<GatewayDeduplicationConfiguration>(config);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

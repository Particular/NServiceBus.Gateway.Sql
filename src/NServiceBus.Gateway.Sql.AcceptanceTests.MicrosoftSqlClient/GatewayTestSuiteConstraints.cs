using NServiceBus.AcceptanceTesting.Support;
using NServiceBus.Gateway.Sql;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.AcceptanceTests
{
    public partial class GatewayTestSuiteConstraints
    {
        public Task<GatewayDeduplicationConfiguration> ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            var connectionString = "Server=hostos;Database=nservicebus;User=sa;Password=NServiceBus!";

            var config = new SqlGatewayDeduplicationConfiguration();
            config.ConnectionBuilder(() => new SqlConnection(connectionString));

            return Task.FromResult<GatewayDeduplicationConfiguration>(config);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

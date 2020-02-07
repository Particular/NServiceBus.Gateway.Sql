#if MicrosoftDataClient
using Microsoft.Data.SqlClient;
#elif SystemDataClient
using System.Data.SqlClient;
#endif
using NServiceBus.AcceptanceTesting.Support;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NServiceBus.Gateway.AcceptanceTests
{
    public partial class GatewayTestSuiteConstraints
    {
        public Task<GatewayDeduplicationConfiguration> ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            var connectionString = DatabaseUtil.GetConnectionString();

            var config = new SqlGatewayDeduplicationConfiguration();
            config.TableName = Regex.Replace(endpointName, "[^A-Za-z0-9]+", "") + "_GatewayDeduplication";
            config.ConnectionBuilder(builder => new SqlConnection(connectionString));

            return Task.FromResult<GatewayDeduplicationConfiguration>(config);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

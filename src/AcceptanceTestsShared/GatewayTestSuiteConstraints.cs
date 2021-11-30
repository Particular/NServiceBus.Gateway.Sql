#if MicrosoftDataClient
using Microsoft.Data.SqlClient;
#elif SystemDataClient
using System.Data.SqlClient;
#endif
using NServiceBus.AcceptanceTesting.Support;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NServiceBus.Configuration.AdvancedExtensibility;

namespace NServiceBus.Gateway.AcceptanceTests
{
    public partial class GatewayTestSuiteConstraints
    {
        public Task ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            var connectionString = DatabaseUtil.GetConnectionString();
            var cfgSettings = configuration.GetSettings();

            endpointName = endpointName ?? cfgSettings.EndpointName();

            var config = new SqlGatewayDeduplicationConfiguration();
            config.TableName = Regex.Replace(endpointName, "[^A-Za-z0-9]+", "") + "_GatewayDeduplication";
            config.ConnectionBuilder(builder => new SqlConnection(connectionString));

            var gatewaySettings = configuration.Gateway(config);
            cfgSettings.Set(gatewaySettings);
            return Task.FromResult(false);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

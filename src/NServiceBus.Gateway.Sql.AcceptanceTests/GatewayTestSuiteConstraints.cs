using NServiceBus.AcceptanceTesting.Support;
using NServiceBus.Gateway.Sql;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.AcceptanceTests
{
    public partial class GatewayTestSuiteConstraints
    {
        public Task ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            var gateway = configuration.Gateway(new SqlGatewayDeduplicationConfiguration
            {
                ConnectionString = "Server=hostos;Database=nservicebus;User=sa;Password=NServiceBus!"
            });

            return Task.FromResult(false);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

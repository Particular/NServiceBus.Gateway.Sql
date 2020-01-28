using NServiceBus.AcceptanceTesting.Support;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.AcceptanceTests
{
    public partial class GatewayTestSuiteConstraints : IGatewayTestSuiteConstraints
    {
        public Task ConfigureDeduplicationStorage(string endpointName, EndpointConfiguration configuration, RunSettings settings)
        {
            return Task.FromResult(false);
        }

        public Task Cleanup()
        {
            return Task.FromResult(false);
        }
    }
}

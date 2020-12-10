using NUnit.Framework;
using Particular.Approvals;
using PublicApiGenerator;

namespace NServiceBus.Gateway.Sql
{
    [TestFixture]
    class ApiApprovals
    {
        [Test]
        public void ApproveNServiceBusGatewaySqlApi()
        {
            var publicApi = typeof(SqlGatewayDeduplicationConfiguration).Assembly.GeneratePublicApi(new ApiGeneratorOptions
            {
                ExcludeAttributes = new[] { "System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute" }
            });
            Approver.Verify(publicApi);
        }
    }
}
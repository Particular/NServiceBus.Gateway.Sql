namespace NServiceBus.Gateway.Sql
{
    using NUnit.Framework;
    using Particular.Approvals;
    using PublicApiGenerator;

    [TestFixture]
    class ApiApprovals
    {
        [Test]
        public void ApproveNServiceBusGatewaySqlApi()
        {
            var publicApi = typeof(SqlGatewayDeduplicationConfiguration).Assembly.GeneratePublicApi(new ApiGeneratorOptions
            {
                ExcludeAttributes = ["System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute"]
            });
            Approver.Verify(publicApi);
        }
    }
}
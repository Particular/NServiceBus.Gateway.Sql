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
            var publicApi = ApiGenerator.GeneratePublicApi(typeof(SqlGatewayDeduplicationConfiguration).Assembly, excludeAttributes: new[] { "System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute" });
            Approver.Verify(publicApi);
        }
    }
}
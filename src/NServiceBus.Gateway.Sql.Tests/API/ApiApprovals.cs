namespace NServiceBus.Gateway.Sql.Tests.API
{
    using NServiceBus.Gateway.Sql;
    using NUnit.Framework;
    using Particular.Approvals;
    using PublicApiGenerator;

    [TestFixture]
    class ApiApprovals
    {
        [Test]
        public void ApproveNServiceBusGatewaySqlApi()
        {

            var publicApi = ApiGenerator.GeneratePublicApi(typeof(SqlGatewayDeduplicationConfiguration).Assembly, excludeAttributes: new[] { "System.Runtime.Versioning.TargetFrameworkAttribute" });
            Approver.Verify(publicApi);
        }
    }
}
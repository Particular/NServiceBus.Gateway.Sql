namespace NServiceBus.Gateway.AcceptanceTests
{
    using NUnit.Framework;

    [SetUpFixture]
    public class SetupFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
#if NETFRAMEWORK
            // Weird bug about deserialization of objects across AppDomains
            // Otherwise it wants test classes to be marked as serializable in netframework
            // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/mitigation-deserialization-of-objects-across-app-domains
            System.Configuration.ConfigurationManager.GetSection("dummy");
#endif

            DatabaseUtil.DropDbIfCollationIncorrect();
            DatabaseUtil.CreateDbIfNotExists();
        }
    }
}

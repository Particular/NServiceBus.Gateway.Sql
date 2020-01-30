using NUnit.Framework;

namespace NServiceBus.Gateway.AcceptanceTests
{
    [SetUpFixture]
    public class SetupFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
            DatabaseUtil.DropDbIfCollationIncorrect();
            DatabaseUtil.CreateDbIfNotExists();
        }
    }
}

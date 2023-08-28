namespace NServiceBus.Gateway.AcceptanceTests
{
    using NUnit.Framework;

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
